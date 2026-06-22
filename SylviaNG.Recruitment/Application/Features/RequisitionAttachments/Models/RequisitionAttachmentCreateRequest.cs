namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models
{
    public class RequisitionAttachmentCreateRequest
    {
        public long RequisitionId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public long? FileSizeBytes { get; set; }
    }
}
