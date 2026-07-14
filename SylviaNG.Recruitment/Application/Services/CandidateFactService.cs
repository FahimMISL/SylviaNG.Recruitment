using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Derives comparable facts (age, total experience, skills, education levels, address text)
    /// from a CandidateProfile. Shared by ShortlistFilterEvaluationService (saved-filter evaluation)
    /// and the ATS dashboard's ad-hoc candidate-attribute filtering (US-050).
    /// </summary>
    public static class CandidateFactService
    {
        public sealed record CandidateFacts(
            int? Age,
            double TotalExperienceYears,
            HashSet<string> SkillNames,
            HashSet<EducationLevelEnum> EducationLevels,
            string AddressText);

        public static CandidateFacts BuildFacts(CandidateProfile? profile)
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

        public static int CalculateAge(DateTime dateOfBirth, DateTime asOf)
        {
            var age = asOf.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > asOf.AddYears(-age))
                age--;
            return age;
        }
    }
}
