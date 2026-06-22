using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class DocumentTemplate : Audit
{
    public long DocumentTemplateId { get; set; }
    public DocumentTypeEnum DocumentType { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PlaceholdersJson { get; set; }
    public int CurrentVersion { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<DocumentTemplateVersion> Versions { get; set; } = new List<DocumentTemplateVersion>();
    public ICollection<GeneratedDocument> GeneratedDocuments { get; set; } = new List<GeneratedDocument>();
}
