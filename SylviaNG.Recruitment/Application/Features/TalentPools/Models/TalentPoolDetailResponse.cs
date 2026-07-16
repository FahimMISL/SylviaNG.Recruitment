using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Models
{
    /// <summary>Pool detail: every member with their latest profile snapshot (US-039 AC3).</summary>
    public class TalentPoolDetailResponse
    {
        public long TalentPoolId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CandidateProfileSummaryResponse> Candidates { get; set; } = new();
    }
}
