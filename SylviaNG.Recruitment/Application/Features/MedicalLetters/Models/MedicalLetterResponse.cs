namespace SylviaNG.Recruitment.Application.Features.MedicalLetters.Models
{
    public class MedicalLetterResponse
    {
        public long MedicalLetterId { get; set; }
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public long OfferLetterId { get; set; }
        public long DocumentTemplateId { get; set; }
        public string DocumentTemplateName { get; set; } = string.Empty;
        public string MedicalTestCenter { get; set; } = string.Empty;
        public string RequiredTests { get; set; } = string.Empty;
        public string FinalBody { get; set; } = string.Empty;
        public string GeneratedPdfPath { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }
}
