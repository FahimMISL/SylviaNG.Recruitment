namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models
{
    public class NotificationTemplateVersionResponse
    {
        public long NotificationTemplateVersionId { get; set; }
        public int VersionNumber { get; set; }
        public string? Subject { get; set; }
        public string Body { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
    }
}
