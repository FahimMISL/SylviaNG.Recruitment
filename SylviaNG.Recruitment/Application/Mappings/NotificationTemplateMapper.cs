using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class NotificationTemplateMapper
    {
        public static NotificationTemplate ToEntity(this NotificationTemplateCreateRequest request)
        {
            return new NotificationTemplate
            {
                Channel = request.Channel,
                Code = request.Code,
                Name = request.Name,
                Subject = request.Subject,
                Body = request.Body,
                IsActive = true,
                CurrentVersionNumber = 1,
            };
        }

        public static NotificationTemplateResponse ToResponse(this NotificationTemplate entity)
        {
            return new NotificationTemplateResponse
            {
                NotificationTemplateId = entity.NotificationTemplateId,
                Channel = entity.Channel,
                Code = entity.Code,
                Name = entity.Name,
                Subject = entity.Subject,
                Body = entity.Body,
                IsActive = entity.IsActive,
                CurrentVersionNumber = entity.CurrentVersionNumber,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }

        public static NotificationTemplateVersionResponse ToResponse(this NotificationTemplateVersion entity)
        {
            return new NotificationTemplateVersionResponse
            {
                NotificationTemplateVersionId = entity.NotificationTemplateVersionId,
                VersionNumber = entity.VersionNumber,
                Subject = entity.Subject,
                Body = entity.Body,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
            };
        }
    }
}
