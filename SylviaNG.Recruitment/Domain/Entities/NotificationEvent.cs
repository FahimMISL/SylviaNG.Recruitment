using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class NotificationEvent : Audit
{
    public long NotificationEventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string? PipelineStage { get; set; }
    public long NotificationTemplateId { get; set; }
    public long? RequisitionId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public NotificationTemplate NotificationTemplate { get; set; } = null!;
    public Requisition? Requisition { get; set; }
}
