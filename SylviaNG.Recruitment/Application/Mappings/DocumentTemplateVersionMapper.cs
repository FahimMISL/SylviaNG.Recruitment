using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class DocumentTemplateVersionMapper
    {
        public static DocumentTemplateVersion ToEntity(this DocumentTemplateVersionCreateRequest request)
        {
            return new DocumentTemplateVersion
            {
                DocumentTemplateId = request.DocumentTemplateId,
                VersionNumber = request.VersionNumber,
                FileUrl = request.FileUrl,
                ChangeNotes = request.ChangeNotes,
            };
        }

        public static void ApplyUpdate(this DocumentTemplateVersion entity, DocumentTemplateVersionUpdateRequest request)
        {
            if (request.DocumentTemplateId.HasValue) entity.DocumentTemplateId = request.DocumentTemplateId.Value;
            if (request.VersionNumber.HasValue) entity.VersionNumber = request.VersionNumber.Value;
            if (request.FileUrl is not null) entity.FileUrl = request.FileUrl;
            if (request.ChangeNotes is not null) entity.ChangeNotes = request.ChangeNotes;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static DocumentTemplateVersionResponse ToResponse(this DocumentTemplateVersion entity)
        {
            return new DocumentTemplateVersionResponse
            {
                DocumentTemplateVersionId = entity.DocumentTemplateVersionId,
                DocumentTemplateId = entity.DocumentTemplateId,
                VersionNumber = entity.VersionNumber,
                FileUrl = entity.FileUrl,
                ChangeNotes = entity.ChangeNotes,
                IsActive = entity.IsActive,
            };
        }
    }
}
