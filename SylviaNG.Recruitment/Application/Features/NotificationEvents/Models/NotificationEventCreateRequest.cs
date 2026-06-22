namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Models
{
    public class NotificationEventCreateRequest
    {
        public string EventName { get; set; } = string.Empty;
        public string? PipelineStage { get; set; }
        public long NotificationTemplateId { get; set; }
        public long? RequisitionId { get; set; }
        public bool IsEnabled { get; set; }
    }
}
