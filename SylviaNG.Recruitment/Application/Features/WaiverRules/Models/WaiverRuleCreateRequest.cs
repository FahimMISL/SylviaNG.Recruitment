using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.WaiverRules.Models
{
    public class WaiverRuleCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WaiverCandidateTypeEnum? CandidateTypeFilter { get; set; }
        public long? SpecialCategoryId { get; set; }
        public long? ReferralSourceId { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
