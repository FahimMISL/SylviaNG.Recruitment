namespace SylviaNG.Recruitment.Application.Features.TalentPools.Models
{
    /// <summary>Pool list row (US-039 AC1 pool list page).</summary>
    public class TalentPoolResponse
    {
        public long TalentPoolId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CandidateCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public long? JobPostingId { get; set; }
        public string? JobPostingTitle { get; set; }
    }
}
