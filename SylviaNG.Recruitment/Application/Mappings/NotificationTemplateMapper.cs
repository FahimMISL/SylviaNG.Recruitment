using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class NotificationTemplateMapper
    {
        public static NotificationTemplate ToEntity(this NotificationTemplateCreateRequest request)
        {
            return new NotificationTemplate
            {
                TemplateName = request.TemplateName,
                Channel = request.Channel,
                Subject = request.Subject,
                Body = request.Body,
                PlaceholdersJson = request.PlaceholdersJson,
            };
        }

        public static void ApplyUpdate(this NotificationTemplate entity, NotificationTemplateUpdateRequest request)
        {
            if (request.TemplateName is not null) entity.TemplateName = request.TemplateName;
            if (request.Channel.HasValue) entity.Channel = request.Channel.Value;
            if (request.Subject is not null) entity.Subject = request.Subject;
            if (request.Body is not null) entity.Body = request.Body;
            if (request.PlaceholdersJson is not null) entity.PlaceholdersJson = request.PlaceholdersJson;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static NotificationTemplateResponse ToResponse(this NotificationTemplate entity)
        {
            return new NotificationTemplateResponse
            {
                NotificationTemplateId = entity.NotificationTemplateId,
                TemplateName = entity.TemplateName,
                Channel = entity.Channel,
                Subject = entity.Subject,
                Body = entity.Body,
                PlaceholdersJson = entity.PlaceholdersJson,
                IsActive = entity.IsActive,
            };
        }
    }
}
