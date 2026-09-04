namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models
{
    public class DocumentTemplateVersionResponse
    {
        public long DocumentTemplateVersionId { get; set; }
        public int VersionNumber { get; set; }
        public string Body { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
    }
}
