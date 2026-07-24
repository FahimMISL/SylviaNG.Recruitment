namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models
{
    public class NotificationTemplatePreviewResponse
    {
        public string? RenderedSubject { get; set; }
        public string RenderedBody { get; set; } = string.Empty;
        public List<string> DetectedPlaceholders { get; set; } = new();
    }
}
