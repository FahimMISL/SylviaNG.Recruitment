namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models
{
    public class RequisitionAttachmentUpdateRequest
    {
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? ContentType { get; set; }
        public long? FileSizeBytes { get; set; }
        public bool? IsActive { get; set; }
    }
}
