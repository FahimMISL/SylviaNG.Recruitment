using ClosedXML.Excel;
using SylviaNG.Recruitment.Application.Features.Analytics.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// EP-14 US-106/US-107: recruitment funnel + time-to-hire analytics, built entirely on top of
    /// ApplicationStatusHistory (already populated on every real status transition - see
    /// JobApplicationService.ApplyStatusChangeAsync/WithdrawApplicationAsync) plus
    /// JobPosting.PostingDate/OfferLetter.DecisionAt. No new schema, no MediatR - a plain
    /// filtered-report service/controller pair, mirroring PaymentReportService/Controller.
    ///
    /// Gap found during implementation: ApplicationStatusHistory only stamps a row on an EXPLICIT
    /// transition - a freshly-submitted application sitting at the default Applied status (the
    /// common case; only waiver-matched submissions get an initial history row, see
    /// JobApplicationService.SubmitAsync) has ZERO history rows. So "reached Applied" is NOT
    /// computed from history like every other funnel stage - every in-scope application counts
    /// toward Applied by definition (ApplicationStatus defaults to Applied at creation).
    /// </summary>
    public class AnalyticsReportService : IAnalyticsReportService
    {
        // EP-14 US-106 AC1: the only real, cross-posting-comparable stage vocabulary is
        // ApplicationStatusEnum - JobApplicationStageProgress.StageName is per-pipeline free text,
        // not comparable across vacancies. Rejected/Withdrawn/AwaitingPayment/DuplicateDismissed
        // are terminal, not funnel rows (see feature doc's scope decision on stage vocabulary).
        private static readonly ApplicationStatusEnum[] FunnelOrder =
        {
            ApplicationStatusEnum.Applied,
            ApplicationStatusEnum.Screening,
            ApplicationStatusEnum.Shortlisted,
            ApplicationStatusEnum.InterviewScheduled,
            ApplicationStatusEnum.Interviewed,
            ApplicationStatusEnum.Offered,
            ApplicationStatusEnum.Hired
        };

        private static readonly Dictionary<ApplicationStatusEnum, string> FunnelLabels = new()
        {
            [ApplicationStatusEnum.Applied] = "Applied",
            [ApplicationStatusEnum.Screening] = "Screened",
            [ApplicationStatusEnum.Shortlisted] = "Shortlisted",
            [ApplicationStatusEnum.InterviewScheduled] = "Interview Scheduled",
            [ApplicationStatusEnum.Interviewed] = "Interviewed",
            [ApplicationStatusEnum.Offered] = "Offered",
            [ApplicationStatusEnum.Hired] = "Hired"
        };

        private static readonly ApplicationStatusEnum[] TerminalDropStatuses =
        {
            ApplicationStatusEnum.Rejected,
            ApplicationStatusEnum.Withdrawn
        };

        // EP-14 US-108: fallback label when a JobApplication has no ReferralSourceId declared -
        // the named channels in the AC (BDJobs/LinkedIn/Employee Referral/Agency) live in the
        // admin-managed ReferralSource lookup, not in ApplicationSourceEnum (see feature doc).
        private static readonly Dictionary<ApplicationSourceEnum, string> FallbackSourceLabels = new()
        {
            [ApplicationSourceEnum.External] = "Career Portal",
            [ApplicationSourceEnum.Internal] = "Internal",
            [ApplicationSourceEnum.Admin] = "Agency/Direct"
        };

        // EP-14 US-110 AC2: fixed 5-band score histogram over WeightedScore (0-100).
        private static readonly (decimal Min, decimal Max, string Label)[] ScoreHistogramBands =
        {
            (0, 20, "0-20"),
            (21, 40, "21-40"),
            (41, 60, "41-60"),
            (61, 80, "61-80"),
            (81, 100, "81-100")
        };

        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IApplicationStatusHistoryRepository _applicationStatusHistoryRepository;
        private readonly IOfferLetterRepository _offerLetterRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IInterviewEvaluationRepository _interviewEvaluationRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public AnalyticsReportService(
            IJobApplicationRepository jobApplicationRepository,
            IApplicationStatusHistoryRepository applicationStatusHistoryRepository,
            IOfferLetterRepository offerLetterRepository,
            ICandidateProfileRepository candidateProfileRepository,
            IInterviewEvaluationRepository interviewEvaluationRepository,
            IEmployeeRepository employeeRepository)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _applicationStatusHistoryRepository = applicationStatusHistoryRepository;
            _offerLetterRepository = offerLetterRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _interviewEvaluationRepository = interviewEvaluationRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<RecruitmentFunnelResponse> GetRecruitmentFunnelAsync(RecruitmentFunnelRequest request)
        {
            var applications = await ScopeApplicationsAsync(
                request.JobPostingId, request.DepartmentId, request.DateFrom, request.DateTo, request.IsInternal);

            var appIds = applications.Select(a => a.JobApplicationId).ToList();
            var history = appIds.Count == 0
                ? new List<ApplicationStatusHistory>()
                : await _applicationStatusHistoryRepository.GetForApplicationsAsync(appIds);

            var historyByApp = history
                .GroupBy(h => h.JobApplicationId)
                .ToDictionary(g => g.Key, g => g.OrderBy(h => h.ChangedAt).ToList());

            var reachedByStage = FunnelOrder.ToDictionary(s => s, _ => new HashSet<long>());
            var highestIndexByApp = new Dictionary<long, int>();
            var finalStatusByApp = new Dictionary<long, ApplicationStatusEnum?>();
            var finalReasonLabelByApp = new Dictionary<long, string?>();

            foreach (var appId in appIds)
            {
                // Every in-scope application has reached Applied by definition - see the gap note
                // in the class docstring, ApplicationStatusHistory does not reliably log it.
                reachedByStage[ApplicationStatusEnum.Applied].Add(appId);
                var highestIndex = 0;

                if (historyByApp.TryGetValue(appId, out var rows) && rows.Count > 0)
                {
                    foreach (var row in rows)
                    {
                        var stageIndex = Array.IndexOf(FunnelOrder, row.ToStatus);
                        if (stageIndex < 0)
                            continue;

                        reachedByStage[row.ToStatus].Add(appId);
                        if (stageIndex > highestIndex)
                            highestIndex = stageIndex;
                    }

                    var last = rows[^1];
                    finalStatusByApp[appId] = last.ToStatus;
                    finalReasonLabelByApp[appId] = last.Reason?.Label;
                }
                else
                {
                    finalStatusByApp[appId] = null;
                    finalReasonLabelByApp[appId] = null;
                }

                highestIndexByApp[appId] = highestIndex;
            }

            var stages = new List<FunnelStageResponse>();
            int? previousCount = null;

            for (var i = 0; i < FunnelOrder.Length; i++)
            {
                var status = FunnelOrder[i];
                var reached = reachedByStage[status];
                var count = reached.Count;

                double? conversion = previousCount is null or 0
                    ? null
                    : Math.Round((double)count / previousCount.Value * 100, 1);

                var nextStatus = i + 1 < FunnelOrder.Length ? FunnelOrder[i + 1] : (ApplicationStatusEnum?)null;
                var passedCount = nextStatus.HasValue ? reached.Intersect(reachedByStage[nextStatus.Value]).Count() : 0;

                var droppedAppIds = reached
                    .Where(id => highestIndexByApp[id] == i
                        && finalStatusByApp.TryGetValue(id, out var finalStatus)
                        && finalStatus.HasValue
                        && TerminalDropStatuses.Contains(finalStatus.Value))
                    .ToList();

                var dropOffReasons = droppedAppIds
                    .Select(id => finalReasonLabelByApp.GetValueOrDefault(id))
                    .Where(label => !string.IsNullOrEmpty(label))
                    .GroupBy(label => label!)
                    .Select(g => new FunnelDropOffReasonResponse { ReasonLabel = g.Key, Count = g.Count() })
                    .OrderByDescending(r => r.Count)
                    .ToList();

                stages.Add(new FunnelStageResponse
                {
                    Status = status,
                    Label = FunnelLabels[status],
                    Count = count,
                    ConversionFromPreviousPercent = conversion,
                    PassedCount = passedCount,
                    DroppedCount = droppedAppIds.Count,
                    DropOffReasons = dropOffReasons
                });

                previousCount = count;
            }

            return new RecruitmentFunnelResponse { Stages = stages };
        }

        public async Task<byte[]> ExportRecruitmentFunnelCsvAsync(RecruitmentFunnelRequest request)
        {
            var funnel = await GetRecruitmentFunnelAsync(request);

            var headers = new[] { "Stage", "Count", "Conversion From Previous (%)", "Passed", "Dropped", "Drop-off Reasons" };
            var rows = funnel.Stages.Select(s => new[]
            {
                s.Label,
                s.Count.ToString(),
                s.ConversionFromPreviousPercent?.ToString("0.0") ?? "",
                s.PassedCount.ToString(),
                s.DroppedCount.ToString(),
                string.Join("; ", s.DropOffReasons.Select(r => $"{r.ReasonLabel} ({r.Count})"))
            }).ToList();

            return CsvWriter.Write(headers, rows);
        }

        public async Task<TimeToHireResponse> GetTimeToHireAsync(TimeToHireRequest request)
        {
            // AC4 filters on JobPosting.PostingDate, not JobApplication.AppliedDate - fetch scoped
            // by JobPosting/Department only, then apply the PostingDate range in memory.
            var applications = await _jobApplicationRepository.GetForAnalyticsScopeAsync(
                request.JobPostingId, request.DepartmentId, null, null);

            var scoped = applications
                .Where(a => request.DateFrom == null || (a.JobPosting.PostingDate != null && a.JobPosting.PostingDate >= request.DateFrom))
                .Where(a => request.DateTo == null || (a.JobPosting.PostingDate != null && a.JobPosting.PostingDate <= request.DateTo))
                .ToList();

            var appIds = scoped.Select(a => a.JobApplicationId).ToList();
            var acceptedOffers = appIds.Count == 0
                ? new List<OfferLetter>()
                : await _offerLetterRepository.GetAcceptedForApplicationsAsync(appIds);

            // AC1/AC2: one time-to-hire sample per accepted offer, not per vacancy - see feature
            // doc's scope decision (a JobPosting with NumberOfPositions > 1 can have >1 hire).
            var samples = acceptedOffers
                .Where(o => o.JobApplication.JobPosting.PostingDate != null)
                .Select(o => new
                {
                    o.JobApplication.JobPostingId,
                    Days = (o.DecisionAt!.Value - o.JobApplication.JobPosting.PostingDate!.Value).TotalDays
                })
                .ToList();

            var response = new TimeToHireResponse
            {
                AverageDays = samples.Count > 0 ? Math.Round(samples.Average(s => s.Days), 1) : null,
                MinDays = samples.Count > 0 ? (int)Math.Round(samples.Min(s => s.Days)) : null,
                MaxDays = samples.Count > 0 ? (int)Math.Round(samples.Max(s => s.Days)) : null,
                VacancyCount = samples.Select(s => s.JobPostingId).Distinct().Count()
            };

            // AC3: per-stage average time, from consecutive ChangedAt deltas across every
            // application that produced an accepted offer in scope.
            var relevantAppIds = acceptedOffers.Select(o => o.JobApplicationId).Distinct().ToList();
            var history = relevantAppIds.Count == 0
                ? new List<ApplicationStatusHistory>()
                : await _applicationStatusHistoryRepository.GetForApplicationsAsync(relevantAppIds);

            var durationsByStatus = new Dictionary<ApplicationStatusEnum, List<double>>();
            foreach (var group in history.GroupBy(h => h.JobApplicationId))
            {
                DateTime? previousChangedAt = null;
                foreach (var row in group.OrderBy(h => h.ChangedAt))
                {
                    if (previousChangedAt != null)
                    {
                        if (!durationsByStatus.TryGetValue(row.ToStatus, out var durations))
                        {
                            durations = new List<double>();
                            durationsByStatus[row.ToStatus] = durations;
                        }

                        durations.Add((row.ChangedAt - previousChangedAt.Value).TotalDays);
                    }

                    previousChangedAt = row.ChangedAt;
                }
            }

            response.StageBreakdown = durationsByStatus
                .Select(kv => new StageDurationResponse
                {
                    ToStatus = kv.Key,
                    AverageDays = Math.Round(kv.Value.Average(), 1),
                    SampleSize = kv.Value.Count
                })
                .OrderBy(s => Array.IndexOf(FunnelOrder, s.ToStatus) is var idx && idx >= 0 ? idx : int.MaxValue)
                .ToList();

            return response;
        }

        public async Task<byte[]> ExportTimeToHireCsvAsync(TimeToHireRequest request)
        {
            var tth = await GetTimeToHireAsync(request);

            var headers = new[] { "Section", "Label", "Average Days", "Sample Size" };
            var rows = new List<string[]>
            {
                new[] { "Summary", "Average Days", tth.AverageDays?.ToString("0.0") ?? "", "" },
                new[] { "Summary", "Min Days", tth.MinDays?.ToString() ?? "", "" },
                new[] { "Summary", "Max Days", tth.MaxDays?.ToString() ?? "", "" },
                new[] { "Summary", "Vacancy Count", tth.VacancyCount.ToString(), "" }
            };

            rows.AddRange(tth.StageBreakdown.Select(s => new[]
            {
                "Stage Breakdown", s.ToStatus.ToString(), s.AverageDays.ToString("0.0"), s.SampleSize.ToString()
            }));

            return CsvWriter.Write(headers, rows);
        }

        public async Task<CandidateSourceAnalyticsResponse> GetCandidateSourceAnalyticsAsync(CandidateSourceAnalyticsRequest request)
        {
            var applications = await _jobApplicationRepository.GetForAnalyticsScopeAsync(
                request.JobPostingId, null, request.DateFrom, request.DateTo);

            var scoped = request.EmploymentType == null
                ? applications
                : applications.Where(a => a.JobPosting.EmploymentType == request.EmploymentType.Value).ToList();

            var shortlistedIndex = Array.IndexOf(FunnelOrder, ApplicationStatusEnum.Shortlisted);

            var segments = scoped
                .GroupBy(a => a.ReferralSource?.Name ?? FallbackSourceLabels.GetValueOrDefault(a.Source, a.Source.ToString()))
                .Select(g =>
                {
                    var total = g.Count();
                    var shortlisted = g.Count(a => Array.IndexOf(FunnelOrder, a.ApplicationStatus) >= shortlistedIndex);
                    var hired = g.Count(a => a.ApplicationStatus == ApplicationStatusEnum.Hired);

                    return new CandidateSourceSegmentResponse
                    {
                        SourceLabel = g.Key,
                        TotalApplications = total,
                        ShortlistedCount = shortlisted,
                        HiredCount = hired,
                        ConversionRatePercent = total > 0 ? Math.Round((double)hired / total * 100, 1) : 0
                    };
                })
                .OrderByDescending(s => s.TotalApplications)
                .ToList();

            return new CandidateSourceAnalyticsResponse { Segments = segments };
        }

        public async Task<byte[]> ExportCandidateSourceAnalyticsExcelAsync(CandidateSourceAnalyticsRequest request)
        {
            var analytics = await GetCandidateSourceAnalyticsAsync(request);

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Candidate Source Analytics");

            var headers = new[] { "Source", "Total Applications", "Shortlisted", "Hired", "Conversion Rate (%)" };
            for (var column = 0; column < headers.Length; column++)
                sheet.Cell(1, column + 1).Value = headers[column];
            sheet.Row(1).Style.Font.Bold = true;

            var rowIndex = 2;
            foreach (var segment in analytics.Segments)
            {
                sheet.Cell(rowIndex, 1).Value = segment.SourceLabel;
                sheet.Cell(rowIndex, 2).Value = segment.TotalApplications;
                sheet.Cell(rowIndex, 3).Value = segment.ShortlistedCount;
                sheet.Cell(rowIndex, 4).Value = segment.HiredCount;
                sheet.Cell(rowIndex, 5).Value = segment.ConversionRatePercent;
                rowIndex++;
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<InterviewAnalyticsResponse> GetInterviewAnalyticsAsync(InterviewAnalyticsRequest request)
        {
            var evaluations = await _interviewEvaluationRepository.GetForAnalyticsScopeAsync(
                request.JobPostingId, request.DepartmentId, request.DateFrom, request.DateTo);

            var scored = evaluations
                .Select(e => new { Evaluation = e, WeightedScore = ComputeWeightedScore(e) })
                .ToList();

            var employeeIds = scored.Select(s => s.Evaluation.EmployeeId).Distinct().ToList();
            var employeeNames = new Dictionary<long, string>();
            foreach (var employeeId in employeeIds)
            {
                var employee = await _employeeRepository.GetByIdAsync(employeeId);
                employeeNames[employeeId] = employee?.EmployeeName ?? $"Employee {employeeId}";
            }

            var panelists = scored
                .GroupBy(s => s.Evaluation.EmployeeId)
                .Select(g => new PanelistAnalyticsResponse
                {
                    EmployeeId = g.Key,
                    EmployeeName = employeeNames.GetValueOrDefault(g.Key, $"Employee {g.Key}"),
                    InterviewsConducted = g.Count(),
                    AverageScore = Math.Round(g.Average(s => s.WeightedScore), 2),
                    RecommendedCount = g.Count(s => s.Evaluation.Recommendation == EvaluationRecommendationEnum.Recommended),
                    NotRecommendedCount = g.Count(s => s.Evaluation.Recommendation == EvaluationRecommendationEnum.NotRecommended),
                    OnHoldCount = g.Count(s => s.Evaluation.Recommendation == EvaluationRecommendationEnum.OnHold)
                })
                .OrderByDescending(p => p.InterviewsConducted)
                .ToList();

            var histogram = ScoreHistogramBands
                .Select(band => new ScoreHistogramBandResponse
                {
                    Band = band.Label,
                    Count = scored.Count(s => s.WeightedScore >= band.Min && s.WeightedScore <= band.Max)
                })
                .ToList();

            return new InterviewAnalyticsResponse { Panelists = panelists, ScoreHistogram = histogram };
        }

        public async Task<byte[]> ExportInterviewAnalyticsExcelAsync(InterviewAnalyticsRequest request)
        {
            var analytics = await GetInterviewAnalyticsAsync(request);

            using var workbook = new XLWorkbook();

            var panelistSheet = workbook.Worksheets.Add("Panelist Stats");
            var panelistHeaders = new[] { "Panelist", "Interviews Conducted", "Average Score (%)", "Recommended", "Not Recommended", "On Hold" };
            for (var column = 0; column < panelistHeaders.Length; column++)
                panelistSheet.Cell(1, column + 1).Value = panelistHeaders[column];
            panelistSheet.Row(1).Style.Font.Bold = true;

            var rowIndex = 2;
            foreach (var panelist in analytics.Panelists)
            {
                panelistSheet.Cell(rowIndex, 1).Value = panelist.EmployeeName;
                panelistSheet.Cell(rowIndex, 2).Value = panelist.InterviewsConducted;
                panelistSheet.Cell(rowIndex, 3).Value = panelist.AverageScore;
                panelistSheet.Cell(rowIndex, 4).Value = panelist.RecommendedCount;
                panelistSheet.Cell(rowIndex, 5).Value = panelist.NotRecommendedCount;
                panelistSheet.Cell(rowIndex, 6).Value = panelist.OnHoldCount;
                rowIndex++;
            }

            panelistSheet.Columns().AdjustToContents();

            var histogramSheet = workbook.Worksheets.Add("Score Histogram");
            histogramSheet.Cell(1, 1).Value = "Band";
            histogramSheet.Cell(1, 2).Value = "Count";
            histogramSheet.Row(1).Style.Font.Bold = true;

            var histogramRow = 2;
            foreach (var band in analytics.ScoreHistogram)
            {
                histogramSheet.Cell(histogramRow, 1).Value = band.Band;
                histogramSheet.Cell(histogramRow, 2).Value = band.Count;
                histogramRow++;
            }

            histogramSheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        /// <summary>Mirrors InterviewEvaluationMapper's DeriveWeightedScore formula (Σ(Score/MaxScore
        /// × Weight) / Σ(Weight) × 100) - duplicated here since that one operates on the response
        /// DTO shape, not the entity, and this runs across many evaluations at once.</summary>
        private static decimal ComputeWeightedScore(InterviewEvaluation evaluation)
        {
            var criteriaById = evaluation.Scorecard.Criteria.ToDictionary(c => c.ScorecardCriterionId);
            var totalWeight = evaluation.Scores.Sum(s => criteriaById.GetValueOrDefault(s.ScorecardCriterionId)?.Weight ?? 0);
            if (totalWeight <= 0) return 0;

            var weightedSum = evaluation.Scores
                .Where(s => (criteriaById.GetValueOrDefault(s.ScorecardCriterionId)?.MaxScore ?? 0) > 0)
                .Sum(s =>
                {
                    var criterion = criteriaById[s.ScorecardCriterionId];
                    return (s.Score / criterion.MaxScore) * criterion.Weight;
                });

            return Math.Round(weightedSum / totalWeight * 100, 2);
        }

        private async Task<List<JobApplication>> ScopeApplicationsAsync(
            long? jobPostingId, long? departmentId, DateTime? dateFrom, DateTime? dateTo, bool? isInternal)
        {
            var applications = await _jobApplicationRepository.GetForAnalyticsScopeAsync(
                jobPostingId, departmentId, dateFrom, dateTo);

            if (isInternal == null)
                return applications;

            var emails = applications
                .Where(a => !string.IsNullOrEmpty(a.CandidateEmail))
                .Select(a => a.CandidateEmail!)
                .Distinct()
                .ToList();

            var profiles = emails.Count == 0
                ? new List<CandidateProfile>()
                : await _candidateProfileRepository.GetByEmailsAsync(emails);
            var profilesByEmail = profiles.ToDictionary(p => p.Email, p => p, StringComparer.OrdinalIgnoreCase);

            return applications
                .Where(a =>
                {
                    var profile = !string.IsNullOrEmpty(a.CandidateEmail) && profilesByEmail.TryGetValue(a.CandidateEmail, out var p)
                        ? p
                        : null;
                    var applicationIsInternal = profile?.IsInternal ?? false;
                    return applicationIsInternal == isInternal.Value;
                })
                .ToList();
        }
    }
}
