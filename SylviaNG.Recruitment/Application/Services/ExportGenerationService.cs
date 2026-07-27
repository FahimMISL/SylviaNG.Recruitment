using System.Text;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// EP-13 US-100: one row per matched application, same fixed column set for every row - same
    /// "single line per candidate" convention CvBankCvBulkExportExcelHandler already established.
    /// Candidate facts prefer the linked CandidateProfile where one exists (richer data: gender,
    /// DOB, education, skills), falling back to the JobApplication's own point-in-time snapshot
    /// fields (CandidateName/Email/Phone) for guest applicants with no profile yet.
    /// </summary>
    public class ExportGenerationService : IExportGenerationService
    {
        private static readonly string[] Headers =
        {
            "Candidate Name", "Email", "Phone", "Job Posting", "Application Status", "Applied Date",
            "Source", "Gender", "Date of Birth", "Highest Education", "Total Experience (Years)", "Skills"
        };

        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;

        public ExportGenerationService(
            IJobApplicationRepository jobApplicationRepository,
            ICandidateProfileRepository candidateProfileRepository)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _candidateProfileRepository = candidateProfileRepository;
        }

        public async Task<ExportFileResult> GenerateCandidateListExportAsync(
            List<long> jobApplicationIds,
            ExportFormatEnum format,
            CancellationToken cancellationToken = default)
        {
            // Synchronous ToList() rather than ToListAsync(): this method only ever runs on
            // ExportRequestWorker's background thread, never an HTTP request thread, and a plain
            // IQueryable<T> (as returned by the generic Query() used in unit tests) doesn't
            // implement IAsyncEnumerable - EF's async LINQ operators require a real EF query
            // provider, which the mocked queryable in ExportGenerationServiceTests isn't.
            // OrderBy with a custom IComparer can't translate to SQL - sort after materializing,
            // same as CvBankCvBulkExportExcelHandler's equivalent in-memory sort.
            var applications = jobApplicationIds.Count == 0
                ? new List<JobApplication>()
                : _jobApplicationRepository.Query()
                    .Include(a => a.JobPosting)
                    .Where(a => jobApplicationIds.Contains(a.JobApplicationId))
                    .ToList()
                    .OrderBy(a => a.CandidateName, StringComparer.OrdinalIgnoreCase)
                    .ToList();

            var profileIds = applications
                .Where(a => a.CandidateProfileId.HasValue)
                .Select(a => a.CandidateProfileId!.Value)
                .Distinct()
                .ToList();

            var profiles = profileIds.Count == 0
                ? new List<CandidateProfile>()
                : await _candidateProfileRepository.GetByIdsWithDetailsAsync(profileIds);

            var profilesById = profiles.ToDictionary(p => p.CandidateProfileId);

            var rows = applications
                .Select(a => BuildRow(a, a.CandidateProfileId.HasValue && profilesById.TryGetValue(a.CandidateProfileId.Value, out var p) ? p : null))
                .ToList();

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");

            return format == ExportFormatEnum.Csv
                ? new ExportFileResult(WriteCsv(rows), "text/csv", $"Candidate-List-Export-{timestamp}.csv", rows.Count)
                : new ExportFileResult(WriteXlsx(rows), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Candidate-List-Export-{timestamp}.xlsx", rows.Count);
        }

        private static string[] BuildRow(JobApplication application, CandidateProfile? profile)
        {
            var facts = CandidateFactService.BuildFacts(profile);
            var topEducation = profile?.Educations.OrderByDescending(e => e.EducationLevel).FirstOrDefault();

            return new[]
            {
                profile?.FullName ?? application.CandidateName,
                profile?.Email ?? application.CandidateEmail ?? string.Empty,
                profile?.Phone ?? application.CandidatePhone ?? string.Empty,
                application.JobPosting?.Title ?? string.Empty,
                application.ApplicationStatus.ToString(),
                application.AppliedDate.HasValue ? application.AppliedDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                application.Source.ToString(),
                profile?.Gender?.Name ?? string.Empty,
                profile?.DateOfBirth.HasValue == true ? profile!.DateOfBirth!.Value.ToString("yyyy-MM-dd") : string.Empty,
                topEducation != null ? $"{topEducation.Degree.Name} - {topEducation.Institution}" : string.Empty,
                profile != null ? Math.Round(facts.TotalExperienceYears, 1).ToString("0.0") : string.Empty,
                profile != null ? string.Join(", ", profile.Skills.Select(s => s.SkillName)) : string.Empty
            };
        }

        private static byte[] WriteXlsx(List<string[]> rows)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Candidate List Export");

            for (var column = 0; column < Headers.Length; column++)
                sheet.Cell(1, column + 1).Value = Headers[column];
            sheet.Row(1).Style.Font.Bold = true;

            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                for (var column = 0; column < Headers.Length; column++)
                    sheet.Cell(rowIndex + 2, column + 1).Value = rows[rowIndex][column];

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static byte[] WriteCsv(List<string[]> rows)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Join(",", Headers.Select(EscapeCsvField)));

            foreach (var row in rows)
                builder.AppendLine(string.Join(",", row.Select(EscapeCsvField)));

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        private static string EscapeCsvField(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";

            return value;
        }
    }
}
