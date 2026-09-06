using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.WaiverRules.Models
{
    public class WaiverRuleResponse
    {
        public long WaiverRuleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WaiverCandidateTypeEnum? CandidateTypeFilter { get; set; }
        public long? SpecialCategoryId { get; set; }
        public string? SpecialCategoryName { get; set; }
        public long? ReferralSourceId { get; set; }
        public string? ReferralSourceName { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
    }
}
