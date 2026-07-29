using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// US-059: generates a per-exam downloadable XLSX score-upload template (prefilled with each
    /// enrollment's id/candidate name for reference) and parses an uploaded XLSX/CSV back into
    /// score updates. Mirrors ExamQuestionImportService's structure exactly (US-054) - same
    /// DocumentFormat.OpenXml usage, same Headers/column-index constants, same row-level error
    /// accumulation with partial-success persistence.
    /// </summary>
    public class ExamScoreImportService : IExamScoreImportService
    {
        private static readonly string[] Headers = { "ExamEnrollmentId", "CandidateName", "Score" };

        private const int ColExamEnrollmentId = 0;
        private const int ColCandidateName = 1;
        private const int ColScore = 2;

        private readonly IExamRepository _examRepository;
        private readonly IExamEnrollmentRepository _examEnrollmentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public ExamScoreImportService(
            IExamRepository examRepository,
            IExamEnrollmentRepository examEnrollmentRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _examRepository = examRepository;
            _examEnrollmentRepository = examEnrollmentRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamScoreImportTemplateResponse> GenerateTemplateAsync(long examId)
        {
            var exam = await _examRepository.GetByIdAsync(examId)
                ?? throw new NotFoundException("Exam", examId);

            var enrollments = await _examEnrollmentRepository.GetByExamIdAsync(examId);

            using var stream = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                var scoresSheetData = new SheetData();
                scoresSheetData.Append(BuildRow(Headers));
                foreach (var enrollment in enrollments)
                {
                    scoresSheetData.Append(BuildRow(new[]
                    {
                        enrollment.ExamEnrollmentId.ToString(),
                        enrollment.JobApplication?.CandidateName ?? string.Empty,
                        string.Empty,
                    }));
                }
                var scoresPart = workbookPart.AddNewPart<WorksheetPart>();
                scoresPart.Worksheet = new Worksheet(scoresSheetData);
                sheets.Append(new Sheet
                {
                    Id = workbookPart.GetIdOfPart(scoresPart),
                    SheetId = 1,
                    Name = "Scores"
                });

                var instructionsSheetData = new SheetData();
                instructionsSheetData.Append(BuildRow(new[] { "Do not edit ExamEnrollmentId or CandidateName", "They identify the row" }));
                instructionsSheetData.Append(BuildRow(new[] { "Score must be between", $"0 and {exam.TotalMarks}" }));
                var instructionsPart = workbookPart.AddNewPart<WorksheetPart>();
                instructionsPart.Worksheet = new Worksheet(instructionsSheetData);
                sheets.Append(new Sheet
                {
                    Id = workbookPart.GetIdOfPart(instructionsPart),
                    SheetId = 2,
                    Name = "Instructions"
                });

                workbookPart.Workbook.Save();
            }

            return new ExamScoreImportTemplateResponse
            {
                Content = stream.ToArray(),
                FileName = $"ExamScoreUploadTemplate-{examId}.xlsx",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

        public async Task<ExamScoreBulkUploadResponse> ImportAsync(long examId, IFormFile file)
        {
            var exam = await _examRepository.GetByIdAsync(examId)
                ?? throw new NotFoundException("Exam", examId);

            var enrollments = await _examEnrollmentRepository.GetByExamIdAsync(examId);
            var enrollmentsById = enrollments.ToDictionary(e => e.ExamEnrollmentId);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var rows = extension switch
            {
                ".xlsx" => ReadXlsxRows(file),
                ".csv" => ReadCsvRows(file),
                _ => throw new ArgumentException($"Unsupported import file type: {extension}. Use .xlsx or .csv.")
            };

            var response = new ExamScoreBulkUploadResponse { TotalRows = rows.Count };
            var scoredByUserName = _currentUserService.GetCurrentUserName();
            var updatedCount = 0;

            for (var i = 0; i < rows.Count; i++)
            {
                var rowNumber = i + 2; // header occupies row 1
                var row = rows[i];

                var idText = Col(row, ColExamEnrollmentId);
                if (!long.TryParse(idText, out var examEnrollmentId))
                {
                    response.Errors.Add(new ExamScoreBulkUploadRowError { RowNumber = rowNumber, Message = "ExamEnrollmentId must be a number." });
                    continue;
                }

                if (!enrollmentsById.TryGetValue(examEnrollmentId, out var enrollment))
                {
                    response.Errors.Add(new ExamScoreBulkUploadRowError { RowNumber = rowNumber, Message = $"ExamEnrollmentId {examEnrollmentId} does not belong to this exam." });
                    continue;
                }

                var scoreText = Col(row, ColScore);
                if (!decimal.TryParse(scoreText, out var score))
                {
                    response.Errors.Add(new ExamScoreBulkUploadRowError { RowNumber = rowNumber, Message = "Score must be a number." });
                    continue;
                }

                if (score < 0 || score > exam.TotalMarks)
                {
                    response.Errors.Add(new ExamScoreBulkUploadRowError { RowNumber = rowNumber, Message = $"Score must be between 0 and the exam's total marks ({exam.TotalMarks})." });
                    continue;
                }

                enrollment.Score = score;
                enrollment.IsPassed = score >= exam.PassMarks;
                enrollment.ScoreSource = ScoreSourceEnum.ManualUpload;
                enrollment.ScoredAt = DateTime.UtcNow;
                enrollment.ScoredByUserName = scoredByUserName;
                _examEnrollmentRepository.Update(enrollment);
                updatedCount++;
            }

            response.FailedCount = response.Errors.Count;
            response.UpdatedCount = updatedCount;

            if (updatedCount > 0)
                await _unitOfWork.SaveChangesAsync();

            return response;
        }

        private static string Col(string[] row, int index) => index < row.Length ? row[index]?.Trim() ?? string.Empty : string.Empty;

        private static Row BuildRow(IEnumerable<string> values)
        {
            var row = new Row();
            foreach (var value in values)
            {
                row.Append(new Cell
                {
                    DataType = CellValues.InlineString,
                    InlineString = new InlineString(new Text(value))
                });
            }
            return row;
        }

        private static List<string[]> ReadXlsxRows(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var document = SpreadsheetDocument.Open(stream, false);

            var workbookPart = document.WorkbookPart!;
            var sheet = workbookPart.Workbook!.Descendants<Sheet>().First();
            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id!.Value!);
            var sharedStringPart = workbookPart.SharedStringTablePart;
            var sharedStrings = sharedStringPart?.SharedStringTable?.Elements<SharedStringItem>().ToList();

            var dataRows = worksheetPart.Worksheet!.Descendants<Row>().Skip(1).ToList();
            var result = new List<string[]>();

            foreach (var row in dataRows)
            {
                var values = new string[Headers.Length];
                foreach (var cell in row.Elements<Cell>())
                {
                    if (cell.CellReference?.Value == null) continue;
                    var columnIndex = GetColumnIndex(cell.CellReference.Value);
                    if (columnIndex < 0 || columnIndex >= values.Length) continue;
                    values[columnIndex] = GetCellText(cell, sharedStrings);
                }

                if (values.All(string.IsNullOrWhiteSpace)) continue;
                result.Add(values);
            }

            return result;
        }

        private static List<string[]> ReadCsvRows(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            var lines = new List<string>();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    lines.Add(line);
            }

            return lines.Skip(1).Select(l => l.Split(',')).ToList();
        }

        private static int GetColumnIndex(string cellReference)
        {
            var columnPart = new string(cellReference.TakeWhile(char.IsLetter).ToArray());
            var index = 0;
            foreach (var ch in columnPart)
            {
                index = index * 26 + (ch - 'A' + 1);
            }
            return index - 1;
        }

        private static string GetCellText(Cell cell, List<SharedStringItem>? sharedStrings)
        {
            if (cell.DataType?.Value == CellValues.InlineString)
            {
                return cell.InlineString?.Text?.Text ?? string.Empty;
            }

            if (cell.CellValue == null) return string.Empty;

            var value = cell.CellValue.InnerText;

            if (cell.DataType?.Value == CellValues.SharedString && sharedStrings != null
                && int.TryParse(value, out var sharedStringIndex)
                && sharedStringIndex >= 0 && sharedStringIndex < sharedStrings.Count)
            {
                return sharedStrings[sharedStringIndex].InnerText;
            }

            return value;
        }
    }
}
