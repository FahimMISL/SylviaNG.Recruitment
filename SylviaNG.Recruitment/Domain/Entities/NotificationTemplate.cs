using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class NotificationTemplate : Audit
{
    public long NotificationTemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public NotificationChannelEnum Channel { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? PlaceholdersJson { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<NotificationEvent> NotificationEvents { get; set; } = new List<NotificationEvent>();
}
