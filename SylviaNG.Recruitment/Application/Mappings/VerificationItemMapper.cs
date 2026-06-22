using SylviaNG.Recruitment.Application.Features.VerificationItems.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class VerificationItemMapper
    {
        public static VerificationItem ToEntity(this VerificationItemCreateRequest request)
        {
            return new VerificationItem
            {
                VerificationWorkflowId = request.VerificationWorkflowId,
                VerificationType = request.VerificationType,
                ItemStatus = request.ItemStatus,
                ReferenceNumber = request.ReferenceNumber,
                Notes = request.Notes,
                EvidenceFileUrl = request.EvidenceFileUrl,
                VerifiedByUserId = request.VerifiedByUserId,
                VerifiedAt = request.VerifiedAt,
            };
        }

        public static void ApplyUpdate(this VerificationItem entity, VerificationItemUpdateRequest request)
        {
            if (request.VerificationWorkflowId.HasValue) entity.VerificationWorkflowId = request.VerificationWorkflowId.Value;
            if (request.VerificationType.HasValue) entity.VerificationType = request.VerificationType.Value;
            if (request.ItemStatus.HasValue) entity.ItemStatus = request.ItemStatus.Value;
            if (request.ReferenceNumber is not null) entity.ReferenceNumber = request.ReferenceNumber;
            if (request.Notes is not null) entity.Notes = request.Notes;
            if (request.EvidenceFileUrl is not null) entity.EvidenceFileUrl = request.EvidenceFileUrl;
            if (request.VerifiedByUserId.HasValue) entity.VerifiedByUserId = request.VerifiedByUserId.Value;
            if (request.VerifiedAt.HasValue) entity.VerifiedAt = request.VerifiedAt;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static VerificationItemResponse ToResponse(this VerificationItem entity)
        {
            return new VerificationItemResponse
            {
                VerificationItemId = entity.VerificationItemId,
                VerificationWorkflowId = entity.VerificationWorkflowId,
                VerificationType = entity.VerificationType,
                ItemStatus = entity.ItemStatus,
                ReferenceNumber = entity.ReferenceNumber,
                Notes = entity.Notes,
                EvidenceFileUrl = entity.EvidenceFileUrl,
                VerifiedByUserId = entity.VerifiedByUserId,
                VerifiedAt = entity.VerifiedAt,
                IsActive = entity.IsActive,
            };
        }
    }
}
