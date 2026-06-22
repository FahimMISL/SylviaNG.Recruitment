using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class DocumentTemplateMapper
    {
        public static DocumentTemplate ToEntity(this DocumentTemplateCreateRequest request)
        {
            return new DocumentTemplate
            {
                DocumentType = request.DocumentType,
                TemplateName = request.TemplateName,
                Description = request.Description,
                PlaceholdersJson = request.PlaceholdersJson,
                CurrentVersion = request.CurrentVersion,
            };
        }

        public static void ApplyUpdate(this DocumentTemplate entity, DocumentTemplateUpdateRequest request)
        {
            if (request.DocumentType.HasValue) entity.DocumentType = request.DocumentType.Value;
            if (request.TemplateName is not null) entity.TemplateName = request.TemplateName;
            if (request.Description is not null) entity.Description = request.Description;
            if (request.PlaceholdersJson is not null) entity.PlaceholdersJson = request.PlaceholdersJson;
            if (request.CurrentVersion.HasValue) entity.CurrentVersion = request.CurrentVersion.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static DocumentTemplateResponse ToResponse(this DocumentTemplate entity)
        {
            return new DocumentTemplateResponse
            {
                DocumentTemplateId = entity.DocumentTemplateId,
                DocumentType = entity.DocumentType,
                TemplateName = entity.TemplateName,
                Description = entity.Description,
                PlaceholdersJson = entity.PlaceholdersJson,
                CurrentVersion = entity.CurrentVersion,
                IsActive = entity.IsActive,
            };
        }
    }
}
