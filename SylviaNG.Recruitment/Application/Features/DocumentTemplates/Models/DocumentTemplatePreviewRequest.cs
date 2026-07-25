namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models
{
    // Takes raw Body text rather than a saved template id so the admin form can preview while
    // still editing, before anything is persisted.
    public class DocumentTemplatePreviewRequest
    {
        public string Body { get; set; } = string.Empty;
        public Dictionary<string, string> PlaceholderValues { get; set; } = new();
    }
}
