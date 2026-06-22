using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ApplicationDuplicateMapper
    {
        public static ApplicationDuplicate ToEntity(this ApplicationDuplicateCreateRequest request)
        {
            return new ApplicationDuplicate
            {
                PrimaryApplicationId = request.PrimaryApplicationId,
                DuplicateApplicationId = request.DuplicateApplicationId,
                MatchField = request.MatchField,
                Resolution = request.Resolution,
                ResolvedByUserId = request.ResolvedByUserId,
                ResolvedAt = request.ResolvedAt,
            };
        }

        public static void ApplyUpdate(this ApplicationDuplicate entity, ApplicationDuplicateUpdateRequest request)
        {
            if (request.PrimaryApplicationId.HasValue) entity.PrimaryApplicationId = request.PrimaryApplicationId.Value;
            if (request.DuplicateApplicationId.HasValue) entity.DuplicateApplicationId = request.DuplicateApplicationId.Value;
            if (request.MatchField is not null) entity.MatchField = request.MatchField;
            if (request.Resolution.HasValue) entity.Resolution = request.Resolution.Value;
            if (request.ResolvedByUserId.HasValue) entity.ResolvedByUserId = request.ResolvedByUserId.Value;
            if (request.ResolvedAt.HasValue) entity.ResolvedAt = request.ResolvedAt;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ApplicationDuplicateResponse ToResponse(this ApplicationDuplicate entity)
        {
            return new ApplicationDuplicateResponse
            {
                ApplicationDuplicateId = entity.ApplicationDuplicateId,
                PrimaryApplicationId = entity.PrimaryApplicationId,
                DuplicateApplicationId = entity.DuplicateApplicationId,
                MatchField = entity.MatchField,
                Resolution = entity.Resolution,
                ResolvedByUserId = entity.ResolvedByUserId,
                ResolvedAt = entity.ResolvedAt,
                IsActive = entity.IsActive,
            };
        }
    }
}
