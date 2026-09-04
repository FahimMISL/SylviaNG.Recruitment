using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JobApplicationDashboardService : IJobApplicationDashboardService
    {
        // EP-14 US-109 AC3: these tracker columns come from a joined stage-progress row, not a
        // native JobApplication property, so PaginationExtensions' generic reflection-based
        // ApplySorting can't translate them to SQL. Sorting by one of these routes through the
        // same in-memory path already established for candidate-attribute filters below.
        private static readonly HashSet<string> StageDerivedSortKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(JobApplicationDashboardResponse.CurrentStageName),
            nameof(JobApplicationDashboardResponse.DaysInCurrentStage),
            nameof(JobApplicationDashboardResponse.IsStale),
            nameof(JobApplicationDashboardResponse.AssignedHrUserName),
            nameof(JobApplicationDashboardResponse.LastUpdatedAt)
        };

        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IJobApplicationStageProgressRepository _jobApplicationStageProgressRepository;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly ICandidateProfileRepository _candidateProfileRepository;

        public JobApplicationDashboardService(
            IJobApplicationRepository jobApplicationRepository,
            IJobApplicationStageProgressRepository jobApplicationStageProgressRepository,
            IApplicationSettingService applicationSettingService,
            ICandidateProfileRepository candidateProfileRepository)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobApplicationStageProgressRepository = jobApplicationStageProgressRepository;
            _applicationSettingService = applicationSettingService;
            _candidateProfileRepository = candidateProfileRepository;
        }

        public async Task<PagedResult<JobApplicationDashboardResponse>> GetDashboardPagedAsync(
            PagedRequest request,
            JobApplicationAttributeFilterRequest filter)
        {
            ValidateAttributeFilterRequest(filter);

            var needsInMemoryPath = filter.HasCandidateAttributeFilters
                || filter.StaleOnly == true
                || (!string.IsNullOrEmpty(request.SortBy) && StageDerivedSortKeys.Contains(request.SortBy));

            if (!needsInMemoryPath)
            {
                var pagedResult = await _jobApplicationRepository.GetPaginatedAllAsync(
                    request, filter.JobPostingId, filter.Status, filter.Source, filter.DateFrom, filter.DateTo);

                var pageResponses = pagedResult.Data.Select(e => e.ToDashboardResponse()).ToList();
                await AttachStageProgressInfoAsync(pageResponses);

                return new PagedResult<JobApplicationDashboardResponse>
                {
                    Data = pageResponses,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }

            // Candidate-attribute filters, StaleOnly, and stage-derived sort keys all require an
            // in-memory pass (email->profile join has no FK; staleness/stage aren't native
            // JobApplication columns) - pagination happens after filtering/sorting here rather
            // than in SQL, same tradeoff already accepted by the candidate-attribute path.
            var matched = filter.HasCandidateAttributeFilters
                ? await GetAttributeFilteredApplicationsAsync(filter)
                : await _jobApplicationRepository.GetAllMatchingAsync(filter.JobPostingId, filter.Status, filter.Source, filter.DateFrom, filter.DateTo);

            var allResponses = matched.Select(e => e.ToDashboardResponse()).ToList();
            await AttachStageProgressInfoAsync(allResponses);

            if (filter.StaleOnly == true)
                allResponses = allResponses.Where(r => r.IsStale).ToList();

            allResponses = ApplyStageDerivedSort(allResponses, request.SortBy, request.SortDirection);

            var page = allResponses
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResult<JobApplicationDashboardResponse>
            {
                Data = page,
                TotalCount = allResponses.Count,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<JobApplicationDetailResponse> GetDetailAsync(long jobApplicationId)
        {
            var entity = await _jobApplicationRepository.GetByIdWithHistoryAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            return entity.ToDetailResponse();
        }

        public async Task<List<long>> GetDashboardMatchingIdsAsync(JobApplicationAttributeFilterRequest filter)
        {
            ValidateAttributeFilterRequest(filter);

            if (filter.StaleOnly == true)
            {
                var matchedForStale = filter.HasCandidateAttributeFilters
                    ? await GetAttributeFilteredApplicationsAsync(filter)
                    : await _jobApplicationRepository.GetAllMatchingAsync(filter.JobPostingId, filter.Status, filter.Source, filter.DateFrom, filter.DateTo);

                var responses = matchedForStale.Select(e => e.ToDashboardResponse()).ToList();
                await AttachStageProgressInfoAsync(responses);
                return responses.Where(r => r.IsStale).Select(r => r.JobApplicationId).ToList();
            }

            if (!filter.HasCandidateAttributeFilters)
                return await _jobApplicationRepository.GetAllMatchingIdsAsync(filter.JobPostingId, filter.Status, filter.Source, filter.DateFrom, filter.DateTo);

            var matched = await GetAttributeFilteredApplicationsAsync(filter);
            return matched.Select(a => a.JobApplicationId).ToList();
        }

        /// <summary>Batch-fills the stage-derived tracker columns (Stage/DaysInCurrentStage/
        /// IsStale/AssignedHR/LastUpdated) on an already-mapped response page, one query for the
        /// whole batch rather than per-row.</summary>
        private async Task AttachStageProgressInfoAsync(List<JobApplicationDashboardResponse> responses)
        {
            if (responses.Count == 0)
                return;

            var ids = responses.Select(r => r.JobApplicationId).ToList();
            var currentByAppId = await _jobApplicationStageProgressRepository.GetCurrentByJobApplicationIdsAsync(ids);
            var defaultStaleDaysThreshold = await _applicationSettingService.GetDefaultStaleDaysThresholdAsync();
            var now = DateTime.UtcNow;

            foreach (var response in responses)
            {
                if (!currentByAppId.TryGetValue(response.JobApplicationId, out var current))
                    continue;

                response.CurrentStageName = current.StageName;
                response.AssignedHrUserName = current.LastUpdatedByUserName;
                // Audit.UpdatedAt is never actually stamped anywhere in this codebase (confirmed -
                // no interceptor, no manual stamp on this entity), so it's always null and useless
                // here. StageEnteredAt is the one real timestamp this row carries - fall back to
                // CompletedAt for rows stamped before StageEnteredAt was backfilled on direct
                // Pending->Completed transitions (see PipelineProgressMapper.ApplyUpdate /
                // JobApplicationStageProgressService.AutoCompleteStage[ByType]Async), so a stage
                // completed in one step without ever going InProgress doesn't show a blank dash.
                var stageAnchor = current.StageEnteredAt ?? current.CompletedAt;
                response.LastUpdatedAt = stageAnchor;

                if (stageAnchor.HasValue)
                {
                    var daysInStage = (int)(now - stageAnchor.Value).TotalDays;
                    response.DaysInCurrentStage = daysInStage;

                    var threshold = current.SlaDaysSnapshot ?? defaultStaleDaysThreshold;
                    response.IsStale = threshold.HasValue && daysInStage > threshold.Value;
                }
            }
        }

        private static List<JobApplicationDashboardResponse> ApplyStageDerivedSort(
            List<JobApplicationDashboardResponse> responses, string? sortBy, string? sortDirection)
        {
            var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(sortBy) || !StageDerivedSortKeys.Contains(sortBy))
                return responses.OrderByDescending(r => r.AppliedDate).ToList();

            IOrderedEnumerable<JobApplicationDashboardResponse> ordered = sortBy.ToLowerInvariant() switch
            {
                "currentstagename" => descending
                    ? responses.OrderByDescending(r => r.CurrentStageName)
                    : responses.OrderBy(r => r.CurrentStageName),
                "daysincurrentstage" => descending
                    ? responses.OrderByDescending(r => r.DaysInCurrentStage)
                    : responses.OrderBy(r => r.DaysInCurrentStage),
                "isstale" => descending
                    ? responses.OrderByDescending(r => r.IsStale)
                    : responses.OrderBy(r => r.IsStale),
                "assignedhrusername" => descending
                    ? responses.OrderByDescending(r => r.AssignedHrUserName)
                    : responses.OrderBy(r => r.AssignedHrUserName),
                "lastupdatedat" => descending
                    ? responses.OrderByDescending(r => r.LastUpdatedAt)
                    : responses.OrderBy(r => r.LastUpdatedAt),
                _ => responses.OrderByDescending(r => r.AppliedDate)
            };

            return ordered.ToList();
        }

        private static void ValidateAttributeFilterRequest(JobApplicationAttributeFilterRequest filter)
        {
            if (filter.HasCandidateAttributeFilters && !filter.JobPostingId.HasValue)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(filter.JobPostingId),
                        "JobPostingId is required when filtering by education, experience, skills, location, or age.")
                });
            }
        }

        private async Task<List<JobApplication>> GetAttributeFilteredApplicationsAsync(JobApplicationAttributeFilterRequest filter)
        {
            var applications = await _jobApplicationRepository.GetAllByJobPostingAndScalarFiltersAsync(
                filter.JobPostingId!.Value, filter.Status, filter.Source, filter.DateFrom, filter.DateTo);

            var emails = applications
                .Select(a => a.CandidateEmail)
                .Where(e => !string.IsNullOrEmpty(e))
                .Distinct()
                .Cast<string>()
                .ToList();

            var profiles = await _candidateProfileRepository.GetByEmailsAsync(emails);
            var profilesByEmail = profiles.ToDictionary(p => p.Email, p => p, StringComparer.OrdinalIgnoreCase);

            var matched = new List<JobApplication>();
            foreach (var application in applications)
            {
                CandidateProfile? profile = null;
                if (!string.IsNullOrEmpty(application.CandidateEmail))
                    profilesByEmail.TryGetValue(application.CandidateEmail, out profile);

                if (profile == null)
                    continue;

                var facts = CandidateFactService.BuildFacts(profile);
                if (MatchesAttributeFilter(facts, filter))
                    matched.Add(application);
            }

            return matched.OrderByDescending(a => a.AppliedDate).ToList();
        }

        public static bool MatchesAttributeFilter(CandidateFactService.CandidateFacts facts, JobApplicationAttributeFilterRequest filter)
        {
            if (filter.MinEducationLevel.HasValue
                && !facts.EducationLevels.Any(l => (int)l >= (int)filter.MinEducationLevel.Value))
                return false;

            if (filter.MinExperienceYears.HasValue && facts.TotalExperienceYears < (double)filter.MinExperienceYears.Value)
                return false;

            if (filter.MaxExperienceYears.HasValue && facts.TotalExperienceYears > (double)filter.MaxExperienceYears.Value)
                return false;

            if (filter.Skills != null && filter.Skills.Count > 0
                && !filter.Skills.Any(skill => facts.SkillNames.Contains(skill)))
                return false;

            if (!string.IsNullOrWhiteSpace(filter.Location)
                && !facts.AddressText.Contains(filter.Location, StringComparison.OrdinalIgnoreCase))
                return false;

            if (filter.MinAge.HasValue && (!facts.Age.HasValue || facts.Age.Value < filter.MinAge.Value))
                return false;

            if (filter.MaxAge.HasValue && (!facts.Age.HasValue || facts.Age.Value > filter.MaxAge.Value))
                return false;

            if (filter.Tags != null && filter.Tags.Count > 0
                && !filter.Tags.Any(tag => facts.TagNames.Contains(tag)))
                return false;

            return true;
        }
    }
}
