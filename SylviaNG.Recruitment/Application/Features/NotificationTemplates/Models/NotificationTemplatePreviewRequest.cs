namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models
{
    // Takes raw Subject/Body text rather than a saved template id so the admin form can preview
    // while still editing, before anything is persisted.
    public class NotificationTemplatePreviewRequest
    {
        public string? Subject { get; set; }
        public string Body { get; set; } = string.Empty;
        public Dictionary<string, string> PlaceholderValues { get; set; } = new();
    }
}
