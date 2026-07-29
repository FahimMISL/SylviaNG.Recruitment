namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models
{
    public class DocumentTemplatePreviewResponse
    {
        public string RenderedBody { get; set; } = string.Empty;
        public List<string> DetectedPlaceholders { get; set; } = new();
    }
}
