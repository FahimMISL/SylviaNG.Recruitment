using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Compares a JobPosting's own eligibility criteria (MinAge/MaxAge/MinEducationLevel/
    /// MinExperienceYears/RequiredDistrict) against a candidate's derived facts (US-024 AC2/AC3).
    /// Same comparison semantics as JobApplicationService.MatchesAttributeFilter (an unknown
    /// candidate age fails an age requirement) but produces a human-readable reason per unmet
    /// requirement instead of a single boolean, since the candidate needs to see why.
    /// </summary>
    public static class JobEligibilityEvaluator
    {
        public static List<string> Evaluate(JobPosting posting, CandidateFactService.CandidateFacts facts)
        {
            var reasons = new List<string>();

            if (posting.MinAge.HasValue && (!facts.Age.HasValue || facts.Age.Value < posting.MinAge.Value))
                reasons.Add($"Minimum age {posting.MinAge.Value} years");

            if (posting.MaxAge.HasValue && (!facts.Age.HasValue || facts.Age.Value > posting.MaxAge.Value))
                reasons.Add($"Maximum age {posting.MaxAge.Value} years");

            if (posting.MinEducationLevel.HasValue
                && !facts.EducationLevels.Any(l => (int)l >= (int)posting.MinEducationLevel.Value))
                reasons.Add($"Minimum education: {posting.MinEducationLevel.Value}");

            if (posting.MinExperienceYears.HasValue && facts.TotalExperienceYears < posting.MinExperienceYears.Value)
                reasons.Add($"Minimum experience: {posting.MinExperienceYears.Value} year(s)");

            if (!string.IsNullOrWhiteSpace(posting.RequiredDistrict)
                && !facts.AddressText.Contains(posting.RequiredDistrict, StringComparison.OrdinalIgnoreCase))
                reasons.Add($"Required district: {posting.RequiredDistrict}");

            return reasons;
        }
    }
}
