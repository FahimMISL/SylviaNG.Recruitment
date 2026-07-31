namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models
{
    public class JoiningBookletResponse
    {
        public long JoiningBookletId { get; set; }
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public long OfferLetterId { get; set; }
        public long DocumentTemplateId { get; set; }
        public string DocumentTemplateName { get; set; } = string.Empty;
        public string BatchLabel { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
        public string RenderedBody { get; set; } = string.Empty;
        public string GeneratedPdfPath { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }
}
