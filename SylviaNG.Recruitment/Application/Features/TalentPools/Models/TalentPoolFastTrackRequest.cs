namespace SylviaNG.Recruitment.Application.Features.TalentPools.Models
{
    /// <summary>
    /// Fast-track pool candidates straight to Shortlisted on a newly-opened vacancy (US-039 AC4).
    /// </summary>
    public class TalentPoolFastTrackRequest
    {
        public List<long> CandidateProfileIds { get; set; } = new();
        public long JobPostingId { get; set; }
    }
}
