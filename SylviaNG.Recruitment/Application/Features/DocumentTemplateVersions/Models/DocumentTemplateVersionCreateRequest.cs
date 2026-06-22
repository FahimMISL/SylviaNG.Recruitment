namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models
{
    public class DocumentTemplateVersionCreateRequest
    {
        public long DocumentTemplateId { get; set; }
        public int VersionNumber { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string? ChangeNotes { get; set; }
    }
}
