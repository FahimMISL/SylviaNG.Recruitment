using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class CandidateDocumentMapper
    {
        public static CandidateDocument ToEntity(this CandidateDocumentCreateRequest request)
        {
            return new CandidateDocument
            {
                CandidateId = request.CandidateId,
                DocumentType = request.DocumentType,
                FileName = request.FileName,
                FileUrl = request.FileUrl,
                ContentType = request.ContentType,
                FileSizeBytes = request.FileSizeBytes,
                FileData = request.FileData,
                IsDefault = request.IsDefault,
            };
        }

        public static void ApplyUpdate(this CandidateDocument entity, CandidateDocumentUpdateRequest request)
        {
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.DocumentType.HasValue) entity.DocumentType = request.DocumentType.Value;
            if (request.FileName is not null) entity.FileName = request.FileName;
            if (request.FileUrl is not null) entity.FileUrl = request.FileUrl;
            if (request.ContentType is not null) entity.ContentType = request.ContentType;
            if (request.FileSizeBytes.HasValue) entity.FileSizeBytes = request.FileSizeBytes.Value;
            if (request.IsDefault.HasValue) entity.IsDefault = request.IsDefault.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static CandidateDocumentResponse ToResponse(this CandidateDocument entity)
        {
            return new CandidateDocumentResponse
            {
                CandidateDocumentId = entity.CandidateDocumentId,
                CandidateId = entity.CandidateId,
                DocumentType = entity.DocumentType,
                FileName = entity.FileName,
                FileUrl = entity.FileUrl,
                ContentType = entity.ContentType,
                FileSizeBytes = entity.FileSizeBytes,
                HasFileData = entity.FileData != null && entity.FileData.Length > 0,
                IsDefault = entity.IsDefault,
                IsActive = entity.IsActive,
            };
        }
    }
}
