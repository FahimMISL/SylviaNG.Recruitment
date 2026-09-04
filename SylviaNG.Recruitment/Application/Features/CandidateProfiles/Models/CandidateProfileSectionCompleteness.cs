namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models
{
    /// <summary>Per-section breakdown backing the completeness ring/badges on the candidate
    /// profile page - each entry reflects the same booleans CalculateCompleteness sums up,
    /// so the UI never shows a percentage the backend didn't actually compute.</summary>
    public class CandidateProfileSectionCompleteness
    {
        public string SectionKey { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public int WeightPercentage { get; set; }
    }
}
