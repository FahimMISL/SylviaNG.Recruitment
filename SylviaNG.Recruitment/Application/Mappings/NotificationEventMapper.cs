using SylviaNG.Recruitment.Application.Features.NotificationEvents.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class NotificationEventMapper
    {
        public static NotificationEvent ToEntity(this NotificationEventCreateRequest request)
        {
            return new NotificationEvent
            {
                EventName = request.EventName,
                PipelineStage = request.PipelineStage,
                NotificationTemplateId = request.NotificationTemplateId,
                RequisitionId = request.RequisitionId,
                IsEnabled = request.IsEnabled,
            };
        }

        public static void ApplyUpdate(this NotificationEvent entity, NotificationEventUpdateRequest request)
        {
            if (request.EventName is not null) entity.EventName = request.EventName;
            if (request.PipelineStage is not null) entity.PipelineStage = request.PipelineStage;
            if (request.NotificationTemplateId.HasValue) entity.NotificationTemplateId = request.NotificationTemplateId.Value;
            if (request.RequisitionId.HasValue) entity.RequisitionId = request.RequisitionId.Value;
            if (request.IsEnabled.HasValue) entity.IsEnabled = request.IsEnabled.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static NotificationEventResponse ToResponse(this NotificationEvent entity)
        {
            return new NotificationEventResponse
            {
                NotificationEventId = entity.NotificationEventId,
                EventName = entity.EventName,
                PipelineStage = entity.PipelineStage,
                NotificationTemplateId = entity.NotificationTemplateId,
                RequisitionId = entity.RequisitionId,
                IsEnabled = entity.IsEnabled,
                IsActive = entity.IsActive,
            };
        }
    }
}
