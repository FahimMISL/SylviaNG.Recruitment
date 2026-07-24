using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class EventTemplateMappingMapper
    {
        public static EventTemplateMapping ToEntity(this EventTemplateMappingCreateRequest request)
        {
            return new EventTemplateMapping
            {
                RecruitmentEvent = request.RecruitmentEvent,
                Channel = request.Channel,
                RecipientType = request.RecipientType,
                NotificationTemplateId = request.NotificationTemplateId,
                IsActive = true,
            };
        }

        public static EventTemplateMappingResponse ToResponse(this EventTemplateMapping entity)
        {
            return new EventTemplateMappingResponse
            {
                EventTemplateMappingId = entity.EventTemplateMappingId,
                RecruitmentEvent = entity.RecruitmentEvent,
                Channel = entity.Channel,
                RecipientType = entity.RecipientType,
                NotificationTemplateId = entity.NotificationTemplateId,
                NotificationTemplateName = entity.NotificationTemplate?.Name ?? string.Empty,
                IsActive = entity.IsActive,
            };
        }
    }
}
