using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-09: an admin-editable notification body for one Channel. Body/Subject hold the CURRENT
/// content - every Update also inserts a NotificationTemplateVersion snapshot so history is never
/// lost. Code is a stable machine key (e.g. "ADMIT_CARD_ISSUED_EMAIL") that EventTemplateMapping
/// and (in Feature 2) dispatch code reference; Name is the admin-facing label.
/// </summary>
public class NotificationTemplate : Audit
{
    public long NotificationTemplateId { get; set; }
    public NotificationChannelEnum Channel { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int CurrentVersionNumber { get; set; } = 1;

    public ICollection<NotificationTemplateVersion> Versions { get; set; } = new List<NotificationTemplateVersion>();
    public ICollection<EventTemplateMapping> EventTemplateMappings { get; set; } = new List<EventTemplateMapping>();
}
