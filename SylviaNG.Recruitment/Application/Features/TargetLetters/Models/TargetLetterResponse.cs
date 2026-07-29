namespace SylviaNG.Recruitment.Application.Features.TargetLetters.Models
{
    public class TargetLetterResponse
    {
        public long TargetLetterId { get; set; }
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public long OfferLetterId { get; set; }
        public long DocumentTemplateId { get; set; }
        public string DocumentTemplateName { get; set; } = string.Empty;
        public string Kpis { get; set; } = string.Empty;
        public string Objectives { get; set; } = string.Empty;
        public string FinalBody { get; set; } = string.Empty;
        public string GeneratedPdfPath { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }
}
