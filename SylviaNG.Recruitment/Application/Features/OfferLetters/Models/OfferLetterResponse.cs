using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Models
{
    public class OfferLetterResponse
    {
        public long OfferLetterId { get; set; }
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public long DocumentTemplateId { get; set; }
        public string DocumentTemplateName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public decimal OfferedSalary { get; set; }
        public DateTime JoiningDate { get; set; }
        public string? ReportingManager { get; set; }
        public DateTime? OfferValidityDate { get; set; }
        public string GeneratedPdfPath { get; set; } = string.Empty;
        public OfferLetterStatusEnum Status { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime? DecisionAt { get; set; }
        public string? DeclineReason { get; set; }
    }
}
