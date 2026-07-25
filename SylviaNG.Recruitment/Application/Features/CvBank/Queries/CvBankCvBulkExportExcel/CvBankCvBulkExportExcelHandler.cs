using ClosedXML.Excel;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankCvBulkExportExcel
{
    /// <summary>
    /// One row per candidate, same fixed column set for every row (US ask: "when Excel file for
    /// bulk amount downloaded, all fields are same for every candidate") - child collections
    /// (education/experience/skills/certifications) are flattened into a single delimited cell
    /// per column rather than expanding into extra rows/columns per candidate.
    /// </summary>
    public class CvBankCvBulkExportExcelHandler : IRequestHandler<CvBankCvBulkExportExcelQuery, CvBankCvFileResponse>
    {
        private static readonly string[] Headers =
        {
            "Full Name", "Email", "Phone", "Gender", "Date of Birth", "Nationality",
            "Present Address", "Highest Education", "All Education", "Total Experience (Years)",
            "Work Experience", "Skills", "Certifications"
        };

        private readonly ICandidateProfileRepository _candidateProfileRepository;

        public CvBankCvBulkExportExcelHandler(ICandidateProfileRepository candidateProfileRepository)
        {
            _candidateProfileRepository = candidateProfileRepository;
        }

        public async Task<CvBankCvFileResponse> Handle(CvBankCvBulkExportExcelQuery query, CancellationToken cancellationToken)
        {
            var candidateProfileIds = query.Request.CandidateProfileIds.Distinct().ToList();
            if (candidateProfileIds.Count == 0)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(CvBankCvBulkRequest.CandidateProfileIds), "Select at least one candidate.")
                });
            }

            var profiles = await _candidateProfileRepository.GetByIdsWithDetailsAsync(candidateProfileIds);

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("CV Bank Export");

            for (var column = 0; column < Headers.Length; column++)
                sheet.Cell(1, column + 1).Value = Headers[column];
            sheet.Row(1).Style.Font.Bold = true;

            var rowIndex = 2;
            foreach (var profile in profiles.OrderBy(p => p.FullName, StringComparer.OrdinalIgnoreCase))
            {
                WriteRow(sheet, rowIndex, profile);
                rowIndex++;
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return new CvBankCvFileResponse
            {
                Content = stream.ToArray(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"CV-Bank-Export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx"
            };
        }

        private static void WriteRow(IXLWorksheet sheet, int rowIndex, CandidateProfile profile)
        {
            var facts = CandidateFactService.BuildFacts(profile);

            var topEducation = profile.Educations.OrderByDescending(e => e.EducationLevel).FirstOrDefault();
            var allEducation = string.Join("; ", profile.Educations.Select(e => $"{e.Degree.Name} - {e.Institution} ({e.PassingYear})"));
            var workExperience = string.Join("; ", profile.WorkExperiences.Select(w =>
                $"{w.Designation} @ {w.CompanyName} ({w.StartDate:MMM yyyy} - {(w.IsCurrent ? "Present" : w.EndDate?.ToString("MMM yyyy") ?? "-")})"));
            var skills = string.Join(", ", profile.Skills.Select(s => s.SkillName));
            var certifications = string.Join(", ", profile.Certifications.Select(c => c.CertificationName));

            sheet.Cell(rowIndex, 1).Value = profile.FullName;
            sheet.Cell(rowIndex, 2).Value = profile.Email;
            sheet.Cell(rowIndex, 3).Value = profile.Phone ?? string.Empty;
            sheet.Cell(rowIndex, 4).Value = profile.Gender?.Name ?? string.Empty;
            sheet.Cell(rowIndex, 5).Value = profile.DateOfBirth.HasValue ? profile.DateOfBirth.Value.ToString("yyyy-MM-dd") : string.Empty;
            sheet.Cell(rowIndex, 6).Value = profile.Nationality ?? string.Empty;
            sheet.Cell(rowIndex, 7).Value = profile.PresentAddressDetail ?? string.Empty;
            sheet.Cell(rowIndex, 8).Value = topEducation != null ? $"{topEducation.Degree.Name} - {topEducation.Institution}" : string.Empty;
            sheet.Cell(rowIndex, 9).Value = allEducation;
            sheet.Cell(rowIndex, 10).Value = Math.Round(facts.TotalExperienceYears, 1);
            sheet.Cell(rowIndex, 11).Value = workExperience;
            sheet.Cell(rowIndex, 12).Value = skills;
            sheet.Cell(rowIndex, 13).Value = certifications;
        }
    }
}
