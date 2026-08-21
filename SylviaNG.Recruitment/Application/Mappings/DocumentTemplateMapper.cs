using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class DocumentTemplateMapper
    {
        public static DocumentTemplate ToEntity(this DocumentTemplateCreateRequest request)
        {
            return new DocumentTemplate
            {
                DocumentType = request.DocumentType,
                Code = request.Code,
                Name = request.Name,
                Body = request.Body,
                IsActive = true,
                CurrentVersionNumber = 1,
            };
        }

        public static DocumentTemplateResponse ToResponse(this DocumentTemplate entity)
        {
            return new DocumentTemplateResponse
            {
                DocumentTemplateId = entity.DocumentTemplateId,
                DocumentType = entity.DocumentType,
                Code = entity.Code,
                Name = entity.Name,
                Body = entity.Body,
                IsActive = entity.IsActive,
                CurrentVersionNumber = entity.CurrentVersionNumber,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }

        public static DocumentTemplateVersionResponse ToResponse(this DocumentTemplateVersion entity)
        {
            return new DocumentTemplateVersionResponse
            {
                DocumentTemplateVersionId = entity.DocumentTemplateVersionId,
                VersionNumber = entity.VersionNumber,
                Body = entity.Body,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
            };
        }
    }
}
