using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Extensions;
using SylviaNG.Recruitment.Application.Features.Dashboard.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IHiringPipelineRepository _hiringPipelineRepository;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IOfferLetterRepository _offerLetterRepository;
        private readonly IJobApplicationStageProgressRepository _jobApplicationStageProgressRepository;
        private readonly IDashboardWidgetConfigService _dashboardWidgetConfigService;
        private readonly ICandidateProfileService _candidateProfileService;

        public DashboardService(
            IHttpContextAccessor httpContextAccessor,
            IJobPostingRepository jobPostingRepository,
            IJobApplicationRepository jobApplicationRepository,
            IHiringPipelineRepository hiringPipelineRepository,
            IInterviewRepository interviewRepository,
            IOfferLetterRepository offerLetterRepository,
            IJobApplicationStageProgressRepository jobApplicationStageProgressRepository,
            IDashboardWidgetConfigService dashboardWidgetConfigService,
            ICandidateProfileService candidateProfileService)
        {
            _httpContextAccessor = httpContextAccessor;
            _jobPostingRepository = jobPostingRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _hiringPipelineRepository = hiringPipelineRepository;
            _interviewRepository = interviewRepository;
            _offerLetterRepository = offerLetterRepository;
            _jobApplicationStageProgressRepository = jobApplicationStageProgressRepository;
            _dashboardWidgetConfigService = dashboardWidgetConfigService;
            _candidateProfileService = candidateProfileService;
        }

        public async Task<DashboardSummaryResponse> GetSummaryAsync()
        {
            var role = GetRole();

            if (role == UserRoleEnum.Candidate)
            {
                var profile = await _candidateProfileService.GetMyProfileAsync();
                return new DashboardSummaryResponse
                {
                    Role = role.ToString(),
                    ProfileCompletenessPercentage = profile.CompletenessPercentage
                };
            }

            var openJobPostingsCount = await _jobPostingRepository.CountByStatusAsync(JobStatusEnum.Open);
            var totalApplicationsCount = await _jobApplicationRepository.CountAllAsync();
            var activeHiringPipelinesCount = await _hiringPipelineRepository.CountActiveAsync();
            var upcomingInterviewsCount = await _interviewRepository.CountUpcomingAsync(DateTime.UtcNow);
            var pendingApprovalsCount = await _jobApplicationStageProgressRepository.CountPendingApprovalsAsync();
            var offersPendingAcceptanceCount = await _offerLetterRepository.CountPendingAcceptanceAsync();
            var applicationsByStatus = await _jobApplicationRepository.GetCountsByStatusAsync();
            var visibleWidgetKeys = await _dashboardWidgetConfigService.GetVisibleWidgetKeysForCurrentRoleAsync();

            return new DashboardSummaryResponse
            {
                Role = role.ToString(),
                OpenJobPostingsCount = openJobPostingsCount,
                TotalApplicationsCount = totalApplicationsCount,
                TotalApplicationsTrend = await BuildApplicationsTrendAsync(totalApplicationsCount),
                TotalApplicationsByStatus = applicationsByStatus.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value),
                ActiveHiringPipelinesCount = activeHiringPipelinesCount,
                UpcomingInterviewsCount = upcomingInterviewsCount,
                PendingApprovalsCount = pendingApprovalsCount,
                OffersPendingAcceptanceCount = offersPendingAcceptanceCount,
                VisibleWidgetKeys = visibleWidgetKeys
            };
        }

        /// <summary>US-105 AC2: week-over-week change for Total Applications. AppliedDate is
        /// monotonic/append-only (applications aren't deleted), so "count as of 7 days ago" safely
        /// reconstructs a real historical value - unlike the mutable-state metrics above, which
        /// have no history table and so carry no trend.</summary>
        private async Task<DashboardMetric> BuildApplicationsTrendAsync(int currentCount)
        {
            var weekAgoCount = await _jobApplicationRepository.CountAppliedAsOfAsync(DateTime.UtcNow.AddDays(-7));

            return new DashboardMetric
            {
                Count = currentCount,
                ChangePercent = weekAgoCount > 0 ? Math.Round((currentCount - weekAgoCount) * 100.0 / weekAgoCount, 1) : null,
                ChangePeriod = "week"
            };
        }

        private UserRoleEnum GetRole()
        {
            var user = _httpContextAccessor.HttpContext?.User
                ?? throw new UnauthorizedAccessException("No authenticated user in the current request.");

            return user.GetHighestRole();
        }
    }
}
