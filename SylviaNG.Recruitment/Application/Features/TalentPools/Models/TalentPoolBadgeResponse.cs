namespace SylviaNG.Recruitment.Application.Features.TalentPools.Models
{
    /// <summary>Pool membership badge shown on a candidate's profile (US-039 AC2).</summary>
    public class TalentPoolBadgeResponse
    {
        public long TalentPoolId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
