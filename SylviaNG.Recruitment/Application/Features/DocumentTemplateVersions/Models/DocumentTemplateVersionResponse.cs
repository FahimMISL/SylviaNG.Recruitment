namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models
{
    public class DocumentTemplateVersionResponse
    {
        public long DocumentTemplateVersionId { get; set; }
        public long DocumentTemplateId { get; set; }
        public int VersionNumber { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string? ChangeNotes { get; set; }
        public bool IsActive { get; set; }
    }
}
