using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ReferralDuplicateMapper
    {
        public static ReferralDuplicate ToEntity(this ReferralDuplicateCreateRequest request)
        {
            return new ReferralDuplicate
            {
                PrimaryReferralId = request.PrimaryReferralId,
                DuplicateReferralId = request.DuplicateReferralId,
                MatchField = request.MatchField,
                Resolution = request.Resolution,
                ResolvedByUserId = request.ResolvedByUserId,
                ResolvedAt = request.ResolvedAt,
            };
        }

        public static void ApplyUpdate(this ReferralDuplicate entity, ReferralDuplicateUpdateRequest request)
        {
            if (request.PrimaryReferralId.HasValue) entity.PrimaryReferralId = request.PrimaryReferralId.Value;
            if (request.DuplicateReferralId.HasValue) entity.DuplicateReferralId = request.DuplicateReferralId.Value;
            if (request.MatchField is not null) entity.MatchField = request.MatchField;
            if (request.Resolution.HasValue) entity.Resolution = request.Resolution.Value;
            if (request.ResolvedByUserId.HasValue) entity.ResolvedByUserId = request.ResolvedByUserId.Value;
            if (request.ResolvedAt.HasValue) entity.ResolvedAt = request.ResolvedAt;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ReferralDuplicateResponse ToResponse(this ReferralDuplicate entity)
        {
            return new ReferralDuplicateResponse
            {
                ReferralDuplicateId = entity.ReferralDuplicateId,
                PrimaryReferralId = entity.PrimaryReferralId,
                DuplicateReferralId = entity.DuplicateReferralId,
                MatchField = entity.MatchField,
                Resolution = entity.Resolution,
                ResolvedByUserId = entity.ResolvedByUserId,
                ResolvedAt = entity.ResolvedAt,
                IsActive = entity.IsActive,
            };
        }
    }
}
