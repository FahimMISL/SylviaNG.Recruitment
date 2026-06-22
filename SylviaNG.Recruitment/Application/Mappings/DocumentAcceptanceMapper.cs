using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class DocumentAcceptanceMapper
    {
        public static DocumentAcceptance ToEntity(this DocumentAcceptanceCreateRequest request)
        {
            return new DocumentAcceptance
            {
                GeneratedDocumentId = request.GeneratedDocumentId,
                CandidateId = request.CandidateId,
                AcceptanceStatus = request.AcceptanceStatus,
                ActionDate = request.ActionDate,
                DeclineReason = request.DeclineReason,
            };
        }

        public static void ApplyUpdate(this DocumentAcceptance entity, DocumentAcceptanceUpdateRequest request)
        {
            if (request.GeneratedDocumentId.HasValue) entity.GeneratedDocumentId = request.GeneratedDocumentId.Value;
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.AcceptanceStatus.HasValue) entity.AcceptanceStatus = request.AcceptanceStatus.Value;
            if (request.ActionDate.HasValue) entity.ActionDate = request.ActionDate;
            if (request.DeclineReason is not null) entity.DeclineReason = request.DeclineReason;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static DocumentAcceptanceResponse ToResponse(this DocumentAcceptance entity)
        {
            return new DocumentAcceptanceResponse
            {
                DocumentAcceptanceId = entity.DocumentAcceptanceId,
                GeneratedDocumentId = entity.GeneratedDocumentId,
                CandidateId = entity.CandidateId,
                AcceptanceStatus = entity.AcceptanceStatus,
                ActionDate = entity.ActionDate,
                DeclineReason = entity.DeclineReason,
                IsActive = entity.IsActive,
            };
        }
    }
}
