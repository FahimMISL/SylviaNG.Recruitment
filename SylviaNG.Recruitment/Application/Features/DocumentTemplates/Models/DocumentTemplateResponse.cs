using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models
{
    public class DocumentTemplateResponse
    {
        public long DocumentTemplateId { get; set; }
        public DocumentTypeEnum DocumentType { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int CurrentVersionNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
