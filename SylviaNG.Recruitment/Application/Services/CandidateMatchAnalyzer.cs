using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Derives provider-agnostic match details (matched skills, experience band) from a
    /// JobPosting + CandidateFacts pair, independent of which IShortlistScoringService produced
    /// the score/explanation (US-037 AC2 - "matched keywords, skill tags, and experience band
    /// shown alongside each score", regardless of Manual or Ai provider).
    /// </summary>
    public static class CandidateMatchAnalyzer
    {
        /// <summary>Candidate skills found (case-insensitive substring) in the posting's Requirements+Description text.</summary>
        public static List<string> GetMatchedSkills(JobPosting jobPosting, CandidateFactService.CandidateFacts facts)
        {
            var requirementsText = $"{jobPosting.Requirements} {jobPosting.Description}";
            if (string.IsNullOrWhiteSpace(requirementsText.Trim()))
                return new List<string>();

            return facts.SkillNames
                .Where(skill => requirementsText.Contains(skill, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>Buckets total years of experience into a human-readable band.</summary>
        public static string GetExperienceBand(double totalExperienceYears)
        {
            return totalExperienceYears switch
            {
                < 1 => "0-1 years",
                < 3 => "1-3 years",
                < 5 => "3-5 years",
                < 10 => "5-10 years",
                _ => "10+ years"
            };
        }
    }
}
