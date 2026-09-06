using SylviaNG.Recruitment.Application.Features.WaiverRules.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class WaiverRuleMapper
    {
        public static WaiverRule ToEntity(this WaiverRuleCreateRequest request)
        {
            return new WaiverRule
            {
                Name = request.Name,
                Description = request.Description,
                CandidateTypeFilter = request.CandidateTypeFilter,
                SpecialCategoryId = request.SpecialCategoryId,
                ReferralSourceId = request.ReferralSourceId,
                Priority = request.Priority,
                IsActive = request.IsActive,
            };
        }

        public static WaiverRuleResponse ToResponse(this WaiverRule entity)
        {
            return new WaiverRuleResponse
            {
                WaiverRuleId = entity.WaiverRuleId,
                Name = entity.Name,
                Description = entity.Description,
                CandidateTypeFilter = entity.CandidateTypeFilter,
                SpecialCategoryId = entity.SpecialCategoryId,
                SpecialCategoryName = entity.SpecialCategory?.Name,
                ReferralSourceId = entity.ReferralSourceId,
                ReferralSourceName = entity.ReferralSource?.Name,
                Priority = entity.Priority,
                IsActive = entity.IsActive,
            };
        }
    }
}
