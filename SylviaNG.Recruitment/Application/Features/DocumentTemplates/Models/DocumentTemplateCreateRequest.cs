using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models
{
    public class DocumentTemplateCreateRequest
    {
        public DocumentTypeEnum DocumentType { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PlaceholdersJson { get; set; }
        public int CurrentVersion { get; set; }
    }
}
