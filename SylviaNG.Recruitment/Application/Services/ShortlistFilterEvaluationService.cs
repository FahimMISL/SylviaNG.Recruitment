using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ShortlistFilterEvaluationService : IShortlistFilterEvaluationService
    {
        private readonly IShortlistFilterRepository _shortlistFilterRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IJobApplicationService _jobApplicationService;
        private readonly IAutoShortlistRunRepository _autoShortlistRunRepository;

        public ShortlistFilterEvaluationService(
            IShortlistFilterRepository shortlistFilterRepository,
            IJobApplicationRepository jobApplicationRepository,
            ICandidateProfileRepository candidateProfileRepository,
            IJobApplicationService jobApplicationService,
            IAutoShortlistRunRepository autoShortlistRunRepository)
        {
            _shortlistFilterRepository = shortlistFilterRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _jobApplicationService = jobApplicationService;
            _autoShortlistRunRepository = autoShortlistRunRepository;
        }

        public async Task<ShortlistFilterPreviewResponse> PreviewAsync(ShortlistFilterPreviewRequest request)
        {
            var (combineWith, criteria) = await ResolveDefinitionAsync(request);
            var (applications, passingIds) = await EvaluateApplicationsAsync(request.JobPostingId, combineWith, criteria);

            return new ShortlistFilterPreviewResponse
            {
                TotalApplications = applications.Count,
                PassingCount = passingIds.Count,
                PassingJobApplicationIds = passingIds
            };
        }

        public async Task<ShortlistFilterApplyResponse> ApplyAsync(ShortlistFilterApplyRequest request)
        {
            var filter = await _shortlistFilterRepository.GetByIdWithCriteriaAsync(request.ShortlistFilterId)
                ?? throw new NotFoundException("ShortlistFilter", request.ShortlistFilterId);

            var criteria = filter.Criteria.Select(c => c.ToCriterionRequest()).ToList();
            var (applications, passingIds) = await EvaluateApplicationsAsync(request.JobPostingId, filter.CombineWith, criteria);

            var bulkResult = await _jobApplicationService.BulkUpdateStatusAsync(new JobApplicationBulkStatusUpdateRequest
            {
                JobApplicationIds = passingIds,
                ToStatus = ApplicationStatusEnum.Shortlisted
            });

            return new ShortlistFilterApplyResponse
            {
                TotalProcessed = applications.Count,
                TotalShortlisted = bulkResult.SucceededIds.Count,
                TotalFailed = bulkResult.Failed.Count,
                Failures = bulkResult.Failed
            };
        }

        private async Task<(List<JobApplication> Applications, List<long> PassingIds)> EvaluateApplicationsAsync(
            long jobPostingId, FilterCombinatorEnum combineWith, List<ShortlistFilterCriterionRequest> criteria)
        {
            var applications = await _jobApplicationRepository.GetAllByJobPostingIdAsync(jobPostingId);

            var emails = applications
                .Select(a => a.CandidateEmail)
                .Where(e => !string.IsNullOrEmpty(e))
                .Distinct()
                .Cast<string>()
                .ToList();

            var profiles = await _candidateProfileRepository.GetByEmailsAsync(emails);
            var profilesByEmail = profiles.ToDictionary(p => p.Email, p => p, StringComparer.OrdinalIgnoreCase);

            // Latest AI/Manual auto-shortlist score per application (US-046), feeding the
            // MinScreeningScore criterion below. Empty dictionary (no run yet) is a normal state,
            // not an error - MinScreeningScore just stays unmet for every application until one exists.
            var latestScores = await _autoShortlistRunRepository.GetLatestScoresByJobPostingIdAsync(jobPostingId);

            var passingIds = new List<long>();
            foreach (var application in applications)
            {
                CandidateProfile? profile = null;
                if (!string.IsNullOrEmpty(application.CandidateEmail))
                    profilesByEmail.TryGetValue(application.CandidateEmail, out profile);

                // No matched profile - every criterion is unmet, documented behavior, not a crash.
                if (profile == null)
                    continue;

                var facts = CandidateFactService.BuildFacts(profile);
                // 0 is a valid score, not "missing" - TryGetValue distinguishes the two.
                int? screeningScore = latestScores.TryGetValue(application.JobApplicationId, out var score) ? score : null;

                if (criteria.Count > 0 && Evaluate(criteria, combineWith, facts, screeningScore))
                    passingIds.Add(application.JobApplicationId);
            }

            return (applications, passingIds);
        }

        private async Task<(FilterCombinatorEnum CombineWith, List<ShortlistFilterCriterionRequest> Criteria)> ResolveDefinitionAsync(ShortlistFilterPreviewRequest request)
        {
            if (request.ShortlistFilterId.HasValue)
            {
                var filter = await _shortlistFilterRepository.GetByIdWithCriteriaAsync(request.ShortlistFilterId.Value)
                    ?? throw new NotFoundException("ShortlistFilter", request.ShortlistFilterId.Value);

                return (filter.CombineWith, filter.Criteria.Select(c => c.ToCriterionRequest()).ToList());
            }

            if (request.Definition != null)
                return (request.Definition.CombineWith, request.Definition.Criteria);

            throw new FluentValidation.ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(
                    nameof(request.ShortlistFilterId), "Either ShortlistFilterId or Definition must be provided.")
            });
        }

        // ── Evaluation ───────────────────────────────────────────────────

        private static bool Evaluate(List<ShortlistFilterCriterionRequest> criteria, FilterCombinatorEnum combineWith, CandidateFactService.CandidateFacts facts, int? screeningScore)
        {
            var results = criteria.Select(c => EvaluateCriterion(c, facts, screeningScore)).ToList();
            return combineWith == FilterCombinatorEnum.And ? results.All(r => r) : results.Any(r => r);
        }

        private static bool EvaluateCriterion(ShortlistFilterCriterionRequest criterion, CandidateFactService.CandidateFacts facts, int? screeningScore)
        {
            switch (criterion.CriterionType)
            {
                case CriterionTypeEnum.EducationLevel:
                    return criterion.MinEducationLevel.HasValue
                        && facts.EducationLevels.Any(l => (int)l >= (int)criterion.MinEducationLevel.Value);

                case CriterionTypeEnum.MinExperienceYears:
                    return criterion.MinExperienceYears.HasValue
                        && facts.TotalExperienceYears >= criterion.MinExperienceYears.Value;

                case CriterionTypeEnum.RequiredSkills:
                    var required = ParseSkillNames(criterion.RequiredSkillNames);
                    return required.Count > 0 && required.All(skill => facts.SkillNames.Contains(skill));

                case CriterionTypeEnum.AgeRange:
                    if (!facts.Age.HasValue || (!criterion.MinAge.HasValue && !criterion.MaxAge.HasValue))
                        return false;
                    if (criterion.MinAge.HasValue && facts.Age.Value < criterion.MinAge.Value)
                        return false;
                    if (criterion.MaxAge.HasValue && facts.Age.Value > criterion.MaxAge.Value)
                        return false;
                    return true;

                case CriterionTypeEnum.District:
                    return !string.IsNullOrWhiteSpace(criterion.RequiredDistrict)
                        && facts.AddressText.Contains(criterion.RequiredDistrict, StringComparison.OrdinalIgnoreCase);

                case CriterionTypeEnum.MinScreeningScore:
                    // Score comes from the latest AutoShortlistRun for this vacancy (US-046).
                    // Unmet if no run has scored this application yet.
                    return criterion.MinScreeningScore.HasValue && screeningScore.HasValue
                        && screeningScore.Value >= criterion.MinScreeningScore.Value;

                default:
                    return false;
            }
        }

        private static List<string> ParseSkillNames(string? delimited)
        {
            if (string.IsNullOrWhiteSpace(delimited))
                return new List<string>();

            return delimited.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        }
    }
}
