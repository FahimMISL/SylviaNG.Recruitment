using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.AutoShortlisting.Models
{
    public class AutoShortlistResultResponse
    {
        public long AutoShortlistResultId { get; set; }
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public int? Score { get; set; }
        public string? Explanation { get; set; }

        /// <summary>Candidate skills matched against the posting's requirements (US-037 AC2).</summary>
        public List<string> MatchedSkills { get; set; } = new();

        /// <summary>Human-readable experience bucket, e.g. "3-5 years" (US-037 AC2).</summary>
        public string? ExperienceBand { get; set; }

        public bool ScoringFailed { get; set; }
        public string? ScoringError { get; set; }
        public bool Passed { get; set; }
        public HrOverrideDecisionEnum? HrOverrideDecision { get; set; }
        public bool FinalIncluded { get; set; }
    }
}
