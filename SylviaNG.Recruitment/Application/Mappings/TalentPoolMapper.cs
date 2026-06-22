using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class TalentPoolMapper
    {
        public static TalentPool ToEntity(this TalentPoolCreateRequest request)
        {
            return new TalentPool
            {
                Name = request.Name,
                Description = request.Description,
                RankingCriteriaJson = request.RankingCriteriaJson,
                CreatedByUserId = request.CreatedByUserId,
            };
        }

        public static void ApplyUpdate(this TalentPool entity, TalentPoolUpdateRequest request)
        {
            if (request.Name is not null) entity.Name = request.Name;
            if (request.Description is not null) entity.Description = request.Description;
            if (request.RankingCriteriaJson is not null) entity.RankingCriteriaJson = request.RankingCriteriaJson;
            if (request.CreatedByUserId.HasValue) entity.CreatedByUserId = request.CreatedByUserId.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static TalentPoolResponse ToResponse(this TalentPool entity)
        {
            return new TalentPoolResponse
            {
                TalentPoolId = entity.TalentPoolId,
                Name = entity.Name,
                Description = entity.Description,
                RankingCriteriaJson = entity.RankingCriteriaJson,
                CreatedByUserId = entity.CreatedByUserId,
                IsActive = entity.IsActive,
            };
        }
    }
}
