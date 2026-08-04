using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// US-054: generates the downloadable XLSX import template and parses uploaded XLSX/CSV
    /// files into ExamQuestion rows. Uses DocumentFormat.OpenXml (already a dependency, used
    /// elsewhere by ResumeParsingService for .docx) rather than adding a new Excel package.
    /// Column order here and in GenerateTemplate must stay in sync - see Headers.
    /// </summary>
    public class ExamQuestionImportService : IExamQuestionImportService
    {
        private static readonly string[] Headers =
        {
            "QuestionText", "QuestionType", "DifficultyLevel", "Marks",
            "Option1Text", "Option1IsCorrect", "Option2Text", "Option2IsCorrect",
            "Option3Text", "Option3IsCorrect", "Option4Text", "Option4IsCorrect",
            "Explanation", "ModelAnswer"
        };

        private const int ColQuestionText = 0;
        private const int ColQuestionType = 1;
        private const int ColDifficultyLevel = 2;
        private const int ColMarks = 3;
        private const int ColOption1Text = 4;
        private const int ColExplanation = 12;
        private const int ColModelAnswer = 13;

        private readonly IExamQuestionRepository _examQuestionRepository;
        private readonly IQuestionGroupRepository _questionGroupRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamQuestionImportService(
            IExamQuestionRepository examQuestionRepository,
            IQuestionGroupRepository questionGroupRepository,
            IUnitOfWork unitOfWork)
        {
            _examQuestionRepository = examQuestionRepository;
            _questionGroupRepository = questionGroupRepository;
            _unitOfWork = unitOfWork;
        }

        public ExamQuestionImportTemplateResponse GenerateTemplate()
        {
            using var stream = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                var questionsSheetData = new SheetData();
                questionsSheetData.Append(BuildRow(Headers));
                var questionsPart = workbookPart.AddNewPart<WorksheetPart>();
                questionsPart.Worksheet = new Worksheet(questionsSheetData);
                sheets.Append(new Sheet
                {
                    Id = workbookPart.GetIdOfPart(questionsPart),
                    SheetId = 1,
                    Name = "Questions"
                });

                var instructionsSheetData = new SheetData();
                instructionsSheetData.Append(BuildRow(new[] { "QuestionType allowed values", string.Join(", ", Enum.GetNames(typeof(QuestionTypeEnum))) }));
                instructionsSheetData.Append(BuildRow(new[] { "DifficultyLevel allowed values", string.Join(", ", Enum.GetNames(typeof(DifficultyLevelEnum))) }));
                instructionsSheetData.Append(BuildRow(new[] { "Option*IsCorrect accepts", "TRUE / FALSE" }));
                instructionsSheetData.Append(BuildRow(new[] { "TrueFalse questions", "Fill Option1Text=True, Option2Text=False, mark exactly one as TRUE" }));
                instructionsSheetData.Append(BuildRow(new[] { "Subjective questions", "Leave all Option columns blank; optionally fill ModelAnswer" }));
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

            return new ExamQuestionImportTemplateResponse
            {
                Content = stream.ToArray(),
                FileName = "ExamQuestionImportTemplate.xlsx",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

        public async Task<ExamQuestionBulkImportResponse> ImportAsync(long questionGroupId, IFormFile file)
        {
            var groupExists = await _questionGroupRepository.GetByIdAsync(questionGroupId) != null;
            if (!groupExists)
                throw new NotFoundException("QuestionGroup", questionGroupId);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var rows = extension switch
            {
                ".xlsx" => ReadXlsxRows(file),
                ".csv" => ReadCsvRows(file),
                _ => throw new ArgumentException($"Unsupported import file type: {extension}. Use .xlsx or .csv.")
            };

            var response = new ExamQuestionBulkImportResponse { TotalRows = rows.Count };
            var validEntities = new List<ExamQuestion>();

            for (var i = 0; i < rows.Count; i++)
            {
                var rowNumber = i + 2; // header occupies row 1
                var (entity, errors) = ParseRow(rows[i], questionGroupId);

                if (errors.Count > 0)
                {
                    response.Errors.Add(new ExamQuestionBulkImportRowError { RowNumber = rowNumber, Message = string.Join("; ", errors) });
                }
                else
                {
                    validEntities.Add(entity!);
                }
            }

            response.FailedCount = response.Errors.Count;
            response.ImportedCount = validEntities.Count;

            if (validEntities.Count > 0)
            {
                await _examQuestionRepository.AddRangeAsync(validEntities);
                await _unitOfWork.SaveChangesAsync();
            }

            return response;
        }

        private static (ExamQuestion? Entity, List<string> Errors) ParseRow(string[] row, long questionGroupId)
        {
            var errors = new List<string>();

            var questionText = Col(row, ColQuestionText);
            if (string.IsNullOrWhiteSpace(questionText))
                errors.Add("QuestionText is required.");

            QuestionTypeEnum? questionType = null;
            var questionTypeText = Col(row, ColQuestionType);
            if (string.IsNullOrWhiteSpace(questionTypeText))
                errors.Add("QuestionType is required.");
            else if (TryParseQuestionType(questionTypeText, out var parsedType))
                questionType = parsedType;
            else
                errors.Add($"QuestionType '{questionTypeText}' is not a recognized value.");

            DifficultyLevelEnum? difficultyLevel = null;
            var difficultyLevelText = Col(row, ColDifficultyLevel);
            if (string.IsNullOrWhiteSpace(difficultyLevelText))
                errors.Add("DifficultyLevel is required.");
            else if (Enum.TryParse<DifficultyLevelEnum>(difficultyLevelText, true, out var parsedDifficulty))
                difficultyLevel = parsedDifficulty;
            else
                errors.Add($"DifficultyLevel '{difficultyLevelText}' is not a recognized value.");

            var marksText = Col(row, ColMarks);
            if (!decimal.TryParse(marksText, out var marks) || marks <= 0)
                errors.Add("Marks must be a positive number.");

            var options = new List<ExamQuestionOptionRequest>();
            for (var optionIndex = 0; optionIndex < 4; optionIndex++)
            {
                var textColumn = ColOption1Text + optionIndex * 2;
                var correctColumn = textColumn + 1;
                var optionText = Col(row, textColumn);
                if (string.IsNullOrWhiteSpace(optionText))
                    continue;

                options.Add(new ExamQuestionOptionRequest
                {
                    OptionText = optionText,
                    IsCorrect = ParseBool(Col(row, correctColumn)),
                    DisplayOrder = options.Count
                });
            }

            if (questionType.HasValue)
                ValidateOptionsForType(questionType.Value, options, errors);

            if (errors.Count > 0)
                return (null, errors);

            var explanation = Col(row, ColExplanation);
            var modelAnswer = Col(row, ColModelAnswer);

            var request = new ExamQuestionCreateRequest
            {
                QuestionGroupId = questionGroupId,
                QuestionText = questionText,
                QuestionType = questionType!.Value,
                DifficultyLevel = difficultyLevel!.Value,
                Marks = marks,
                Explanation = string.IsNullOrWhiteSpace(explanation) ? null : explanation,
                ModelAnswer = string.IsNullOrWhiteSpace(modelAnswer) ? null : modelAnswer,
                Options = options
            };

            return (request.ToEntity(), errors);
        }

        private static void ValidateOptionsForType(QuestionTypeEnum questionType, List<ExamQuestionOptionRequest> options, List<string> errors)
        {
            switch (questionType)
            {
                case QuestionTypeEnum.Subjective:
                    if (options.Count > 0)
                        errors.Add("Subjective questions must not have any options.");
                    break;

                case QuestionTypeEnum.TrueFalse:
                    if (options.Count != 2)
                    {
                        errors.Add("True/False questions must have exactly 2 options (True and False).");
                        break;
                    }
                    var hasTrue = options.Any(o => o.OptionText.Equals("True", StringComparison.OrdinalIgnoreCase));
                    var hasFalse = options.Any(o => o.OptionText.Equals("False", StringComparison.OrdinalIgnoreCase));
                    if (!hasTrue || !hasFalse)
                        errors.Add("True/False questions must have options named 'True' and 'False'.");
                    if (options.Count(o => o.IsCorrect) != 1)
                        errors.Add("True/False questions must have exactly 1 correct option.");
                    break;

                case QuestionTypeEnum.McqSingle:
                    if (options.Count < 2)
                        errors.Add("MCQ (single correct) questions need at least 2 options.");
                    if (options.Count(o => o.IsCorrect) != 1)
                        errors.Add("MCQ (single correct) questions must have exactly 1 correct option.");
                    break;

                case QuestionTypeEnum.McqMultiple:
                    if (options.Count < 2)
                        errors.Add("MCQ (multiple correct) questions need at least 2 options.");
                    if (options.Count(o => o.IsCorrect) < 1)
                        errors.Add("MCQ (multiple correct) questions must have at least 1 correct option.");
                    break;
            }
        }

        private static string Col(string[] row, int index) => index < row.Length ? row[index]?.Trim() ?? string.Empty : string.Empty;

        private static bool ParseBool(string text) => text.Trim().ToUpperInvariant() is "TRUE" or "1" or "YES" or "Y";

        // Accepts the raw enum name (documented in the template's Instructions sheet) and also
        // the human-friendly labels shown in the app's own "Add Question" dropdown - HR filling
        // the sheet by hand naturally reaches for what they see on screen rather than the enum
        // name, so both need to work.
        private static readonly Dictionary<string, QuestionTypeEnum> QuestionTypeAliases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["MCQ (Single Correct)"] = QuestionTypeEnum.McqSingle,
            ["MCQ Single"] = QuestionTypeEnum.McqSingle,
            ["MCQ (Multiple Correct)"] = QuestionTypeEnum.McqMultiple,
            ["MCQ Multiple"] = QuestionTypeEnum.McqMultiple,
            ["True/False"] = QuestionTypeEnum.TrueFalse,
        };

        private static bool TryParseQuestionType(string text, out QuestionTypeEnum questionType)
        {
            if (Enum.TryParse(text, true, out questionType))
                return true;

            if (QuestionTypeAliases.TryGetValue(text.Trim(), out var aliased))
            {
                questionType = aliased;
                return true;
            }

            return false;
        }

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

            // First non-empty line is the header - skip it, same as the xlsx path.
            return lines.Skip(1).Select(SplitCsvLine).ToList();
        }

        private static string[] SplitCsvLine(string line)
        {
            var fields = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"') { current.Append('"'); i++; }
                        else inQuotes = false;
                    }
                    else
                    {
                        current.Append(c);
                    }
                }
                else
                {
                    if (c == '"') inQuotes = true;
                    else if (c == ',') { fields.Add(current.ToString()); current.Clear(); }
                    else current.Append(c);
                }
            }
            fields.Add(current.ToString());
            return fields.ToArray();
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
            // Inline strings (t="inlineStr") store their text under <is><t>, not <v> - this is
            // what our own GenerateTemplate() writer uses, and what Excel keeps using for cells
            // that started out inline-string-typed even after the user edits and re-saves.
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
