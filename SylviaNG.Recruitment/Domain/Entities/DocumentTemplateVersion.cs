using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class DocumentTemplateVersion : Audit
{
    public long DocumentTemplateVersionId { get; set; }
    public long DocumentTemplateId { get; set; }
    public int VersionNumber { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string? ChangeNotes { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public DocumentTemplate DocumentTemplate { get; set; } = null!;
}
