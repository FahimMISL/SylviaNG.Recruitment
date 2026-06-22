namespace SylviaNG.Recruitment.Application.Features.TalentPools.Models
{
    public class TalentPoolCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? RankingCriteriaJson { get; set; }
        public long CreatedByUserId { get; set; }
    }
}
