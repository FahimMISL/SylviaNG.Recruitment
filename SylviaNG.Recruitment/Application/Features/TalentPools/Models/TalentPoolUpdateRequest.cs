namespace SylviaNG.Recruitment.Application.Features.TalentPools.Models
{
    public class TalentPoolUpdateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? RankingCriteriaJson { get; set; }
        public long? CreatedByUserId { get; set; }
        public bool? IsActive { get; set; }
    }
}
