using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class GeneratedDocumentMapper
    {
        public static GeneratedDocument ToEntity(this GeneratedDocumentCreateRequest request)
        {
            return new GeneratedDocument
            {
                DocumentTemplateId = request.DocumentTemplateId,
                CandidateId = request.CandidateId,
                JobApplicationId = request.JobApplicationId,
                FileUrl = request.FileUrl,
                FileName = request.FileName,
                FileFormat = request.FileFormat,
                GeneratedAt = request.GeneratedAt,
                GeneratedByUserId = request.GeneratedByUserId,
            };
        }

        public static void ApplyUpdate(this GeneratedDocument entity, GeneratedDocumentUpdateRequest request)
        {
            if (request.DocumentTemplateId.HasValue) entity.DocumentTemplateId = request.DocumentTemplateId.Value;
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.JobApplicationId.HasValue) entity.JobApplicationId = request.JobApplicationId.Value;
            if (request.FileUrl is not null) entity.FileUrl = request.FileUrl;
            if (request.FileName is not null) entity.FileName = request.FileName;
            if (request.FileFormat is not null) entity.FileFormat = request.FileFormat;
            if (request.GeneratedAt.HasValue) entity.GeneratedAt = request.GeneratedAt.Value;
            if (request.GeneratedByUserId.HasValue) entity.GeneratedByUserId = request.GeneratedByUserId.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static GeneratedDocumentResponse ToResponse(this GeneratedDocument entity)
        {
            return new GeneratedDocumentResponse
            {
                GeneratedDocumentId = entity.GeneratedDocumentId,
                DocumentTemplateId = entity.DocumentTemplateId,
                CandidateId = entity.CandidateId,
                JobApplicationId = entity.JobApplicationId,
                FileUrl = entity.FileUrl,
                FileName = entity.FileName,
                FileFormat = entity.FileFormat,
                GeneratedAt = entity.GeneratedAt,
                GeneratedByUserId = entity.GeneratedByUserId,
                IsActive = entity.IsActive,
            };
        }
    }
}
