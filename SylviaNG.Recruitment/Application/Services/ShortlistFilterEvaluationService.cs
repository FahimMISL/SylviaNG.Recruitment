using SylviaNG.Recruitment.Application.Common.Exceptions;
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

        public ShortlistFilterEvaluationService(
            IShortlistFilterRepository shortlistFilterRepository,
            IJobApplicationRepository jobApplicationRepository,
            ICandidateProfileRepository candidateProfileRepository)
        {
            _shortlistFilterRepository = shortlistFilterRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _candidateProfileRepository = candidateProfileRepository;
        }

        public async Task<ShortlistFilterPreviewResponse> PreviewAsync(ShortlistFilterPreviewRequest request)
        {
            var (combineWith, criteria) = await ResolveDefinitionAsync(request);
            var applications = await _jobApplicationRepository.GetAllByJobPostingIdAsync(request.JobPostingId);

            var emails = applications
                .Select(a => a.CandidateEmail)
                .Where(e => !string.IsNullOrEmpty(e))
                .Distinct()
                .Cast<string>()
                .ToList();

            var profiles = await _candidateProfileRepository.GetByEmailsAsync(emails);
            var profilesByEmail = profiles.ToDictionary(p => p.Email, p => p, StringComparer.OrdinalIgnoreCase);

            var passingIds = new List<long>();
            foreach (var application in applications)
            {
                CandidateProfile? profile = null;
                if (!string.IsNullOrEmpty(application.CandidateEmail))
                    profilesByEmail.TryGetValue(application.CandidateEmail, out profile);

                // No matched profile - every criterion is unmet, documented behavior, not a crash.
                if (profile == null)
                    continue;

                var facts = BuildFacts(profile);

                if (criteria.Count > 0 && Evaluate(criteria, combineWith, facts))
                    passingIds.Add(application.JobApplicationId);
            }

            return new ShortlistFilterPreviewResponse
            {
                TotalApplications = applications.Count,
                PassingCount = passingIds.Count,
                PassingJobApplicationIds = passingIds
            };
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

        // ── Candidate fact derivation ───────────────────────────────────

        private sealed record CandidateFacts(
            int? Age,
            double TotalExperienceYears,
            HashSet<string> SkillNames,
            HashSet<EducationLevelEnum> EducationLevels,
            string AddressText);

        private static CandidateFacts BuildFacts(CandidateProfile? profile)
        {
            if (profile == null)
            {
                return new CandidateFacts(null, 0, new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                    new HashSet<EducationLevelEnum>(), string.Empty);
            }

            var age = profile.DateOfBirth.HasValue ? CalculateAge(profile.DateOfBirth.Value, DateTime.UtcNow) : (int?)null;

            var totalExperienceYears = profile.WorkExperiences?
                .Sum(w => ((w.EndDate ?? DateTime.UtcNow) - w.StartDate).TotalDays / 365.25) ?? 0;

            var skillNames = new HashSet<string>(
                profile.Skills?.Select(s => s.SkillName).Where(n => !string.IsNullOrWhiteSpace(n)) ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);

            var educationLevels = new HashSet<EducationLevelEnum>(
                profile.Educations?.Where(e => e.EducationLevel.HasValue).Select(e => e.EducationLevel!.Value) ?? Enumerable.Empty<EducationLevelEnum>());

            var addressText = string.Join(" ", new[] { profile.PresentAddress, profile.PermanentAddress }.Where(a => !string.IsNullOrWhiteSpace(a)));

            return new CandidateFacts(age, totalExperienceYears, skillNames, educationLevels, addressText);
        }

        private static int CalculateAge(DateTime dateOfBirth, DateTime asOf)
        {
            var age = asOf.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > asOf.AddYears(-age))
                age--;
            return age;
        }

        // ── Evaluation ───────────────────────────────────────────────────

        private static bool Evaluate(List<ShortlistFilterCriterionRequest> criteria, FilterCombinatorEnum combineWith, CandidateFacts facts)
        {
            var results = criteria.Select(c => EvaluateCriterion(c, facts)).ToList();
            return combineWith == FilterCombinatorEnum.And ? results.All(r => r) : results.Any(r => r);
        }

        private static bool EvaluateCriterion(ShortlistFilterCriterionRequest criterion, CandidateFacts facts)
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
                    // No screening-score field exists anywhere yet (AI resume screening is a
                    // separate, unbuilt story) - always unmet until that lands. Documented gap,
                    // not a bug: do not "fix" this without wiring a real score source first.
                    return false;

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
