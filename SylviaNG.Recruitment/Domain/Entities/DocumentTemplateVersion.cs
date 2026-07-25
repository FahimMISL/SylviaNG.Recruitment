using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-10: append-only snapshot of a DocumentTemplate's Body taken on every Create/Update.
/// Audit's CreatedAt/CreatedBy double as this snapshot's edited-at/edited-by, same convention as
/// NotificationTemplateVersion.
/// </summary>
public class DocumentTemplateVersion : Audit
{
    public long DocumentTemplateVersionId { get; set; }
    public long DocumentTemplateId { get; set; }
    public int VersionNumber { get; set; }
    public string Body { get; set; } = string.Empty;

    public DocumentTemplate DocumentTemplate { get; set; } = null!;
}
