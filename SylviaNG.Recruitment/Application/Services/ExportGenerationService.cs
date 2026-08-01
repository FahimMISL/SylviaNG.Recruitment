using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Common.Utilities;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Utils;

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

        // EP-14 US-109 AC1/AC5: tracker export columns.
        private static readonly string[] TrackerHeaders =
        {
            "Vacancy", "Candidate Name", "Stage", "Status", "Last Updated", "Days in Current Stage", "Stale", "Assigned HR"
        };

        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IJobApplicationStageProgressRepository _jobApplicationStageProgressRepository;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly ICvPdfGeneratorService _cvPdfGeneratorService;

        public ExportGenerationService(
            IJobApplicationRepository jobApplicationRepository,
            ICandidateProfileRepository candidateProfileRepository,
            IJobApplicationStageProgressRepository jobApplicationStageProgressRepository,
            IApplicationSettingService applicationSettingService,
            ICvPdfGeneratorService cvPdfGeneratorService)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _jobApplicationStageProgressRepository = jobApplicationStageProgressRepository;
            _applicationSettingService = applicationSettingService;
            _cvPdfGeneratorService = cvPdfGeneratorService;
        }

        public async Task<ExportFileResult> GenerateBulkCvZipAsync(
            List<long> jobApplicationIds,
            CancellationToken cancellationToken = default)
        {
            var applications = jobApplicationIds.Count == 0
                ? new List<JobApplication>()
                : _jobApplicationRepository.Query()
                    .Where(a => jobApplicationIds.Contains(a.JobApplicationId))
                    .ToList();

            var profileIds = applications
                .Where(a => a.CandidateProfileId.HasValue)
                .Select(a => a.CandidateProfileId!.Value)
                .Distinct()
                .ToList();

            var profilesById = profileIds.Count == 0
                ? new Dictionary<long, CandidateProfile>()
                : (await _candidateProfileRepository.GetByIdsWithDetailsAsync(profileIds)).ToDictionary(p => p.CandidateProfileId);

            var items = applications
                .Where(a => a.CandidateProfileId.HasValue && profilesById.ContainsKey(a.CandidateProfileId.Value))
                .Select(a => (a.JobApplicationId, a.CandidateName, profilesById[a.CandidateProfileId!.Value]))
                .ToList();

            var content = await CvZipBuilder.BuildAsync(items, _cvPdfGeneratorService, cancellationToken);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");

            return new ExportFileResult(content, "application/zip", $"Candidate-CVs-{timestamp}.zip", items.Count);
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
                ? new ExportFileResult(CsvWriter.Write(Headers, rows), "text/csv", $"Candidate-List-Export-{timestamp}.csv", rows.Count)
                : new ExportFileResult(WriteXlsx(Headers, "Candidate List Export", rows), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Candidate-List-Export-{timestamp}.xlsx", rows.Count);
        }

        public async Task<ExportFileResult> GenerateJobApplicationTrackerExportAsync(
            List<long> jobApplicationIds,
            ExportFormatEnum format,
            CancellationToken cancellationToken = default)
        {
            var applications = jobApplicationIds.Count == 0
                ? new List<JobApplication>()
                : _jobApplicationRepository.Query()
                    .Include(a => a.JobPosting)
                    .Where(a => jobApplicationIds.Contains(a.JobApplicationId))
                    .ToList()
                    .OrderByDescending(a => a.AppliedDate)
                    .ToList();

            var currentByAppId = await _jobApplicationStageProgressRepository.GetCurrentByJobApplicationIdsAsync(
                applications.Select(a => a.JobApplicationId).ToList());
            var defaultStaleDaysThreshold = await _applicationSettingService.GetDefaultStaleDaysThresholdAsync();
            var now = DateTime.UtcNow;

            var rows = applications
                .Select(a => BuildTrackerRow(a, currentByAppId.GetValueOrDefault(a.JobApplicationId), defaultStaleDaysThreshold, now))
                .ToList();

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");

            return format == ExportFormatEnum.Csv
                ? new ExportFileResult(CsvWriter.Write(TrackerHeaders, rows), "text/csv", $"Job-Application-Tracker-{timestamp}.csv", rows.Count)
                : new ExportFileResult(WriteXlsx(TrackerHeaders, "Job Application Tracker", rows), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Job-Application-Tracker-{timestamp}.xlsx", rows.Count);
        }

        private static string[] BuildTrackerRow(
            JobApplication application, JobApplicationStageProgress? currentStage, int? defaultStaleDaysThreshold, DateTime now)
        {
            int? daysInStage = currentStage?.StageEnteredAt.HasValue == true
                ? (int)(now - currentStage.StageEnteredAt!.Value).TotalDays
                : null;
            var threshold = currentStage?.SlaDaysSnapshot ?? defaultStaleDaysThreshold;
            var isStale = daysInStage.HasValue && threshold.HasValue && daysInStage.Value > threshold.Value;

            return new[]
            {
                application.JobPosting?.Title ?? string.Empty,
                application.CandidateName,
                currentStage?.StageName ?? string.Empty,
                application.ApplicationStatus.ToString(),
                currentStage?.StageEnteredAt.HasValue == true ? currentStage.StageEnteredAt!.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty,
                daysInStage?.ToString() ?? string.Empty,
                isStale ? "Yes" : "No",
                currentStage?.LastUpdatedByUserName ?? string.Empty
            };
        }

        private static string[] BuildRow(JobApplication application, CandidateProfile? profile)
        {
            var facts = CandidateFactService.BuildFacts(profile);
            var topEducation = profile?.Educations.OrderByDescending(e => e.EducationLevel).FirstOrDefault();

            return new[]
            {
                profile?.FullName ?? application.CandidateName,
                profile?.Email ?? application.CandidateEmail ?? string.Empty,
                profile?.Phone != null ? $"{(profile.Country?.DialCode != null ? profile.Country.DialCode + " " : string.Empty)}{profile.Phone}" : application.CandidatePhone ?? string.Empty,
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

        private static byte[] WriteXlsx(string[] headers, string sheetName, List<string[]> rows)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add(sheetName);

            for (var column = 0; column < headers.Length; column++)
                sheet.Cell(1, column + 1).Value = headers[column];
            sheet.Row(1).Style.Font.Bold = true;

            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                for (var column = 0; column < headers.Length; column++)
                    sheet.Cell(rowIndex + 2, column + 1).Value = rows[rowIndex][column];

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

    }
}
