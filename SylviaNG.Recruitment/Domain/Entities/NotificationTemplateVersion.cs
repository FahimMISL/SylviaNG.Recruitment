using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-09: append-only snapshot of a NotificationTemplate's Subject/Body taken on every Create/Update.
/// Audit's CreatedAt/CreatedBy double as this snapshot's edited-at/edited-by - there is no separate
/// "who edited" column. No pre-existing content-versioning pattern in the codebase to copy; this
/// establishes it, borrowing ApplicationStatusHistory's FK+cascade shape structurally only.
/// </summary>
public class NotificationTemplateVersion : Audit
{
    public long NotificationTemplateVersionId { get; set; }
    public long NotificationTemplateId { get; set; }
    public int VersionNumber { get; set; }
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;

    public NotificationTemplate NotificationTemplate { get; set; } = null!;
}
