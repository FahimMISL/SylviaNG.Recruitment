using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models
{
    public class DocumentTemplateResponse
    {
        public long DocumentTemplateId { get; set; }
        public DocumentTypeEnum DocumentType { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PlaceholdersJson { get; set; }
        public int CurrentVersion { get; set; }
        public bool IsActive { get; set; }
    }
}
