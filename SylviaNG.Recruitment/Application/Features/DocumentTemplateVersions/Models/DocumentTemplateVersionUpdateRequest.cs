namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models
{
    public class DocumentTemplateVersionUpdateRequest
    {
        public long? DocumentTemplateId { get; set; }
        public int? VersionNumber { get; set; }
        public string? FileUrl { get; set; }
        public string? ChangeNotes { get; set; }
        public bool? IsActive { get; set; }
    }
}
