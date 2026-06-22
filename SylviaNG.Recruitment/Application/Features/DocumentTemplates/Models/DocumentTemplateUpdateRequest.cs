using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models
{
    public class DocumentTemplateUpdateRequest
    {
        public DocumentTypeEnum? DocumentType { get; set; }
        public string? TemplateName { get; set; }
        public string? Description { get; set; }
        public string? PlaceholdersJson { get; set; }
        public int? CurrentVersion { get; set; }
        public bool? IsActive { get; set; }
    }
}
