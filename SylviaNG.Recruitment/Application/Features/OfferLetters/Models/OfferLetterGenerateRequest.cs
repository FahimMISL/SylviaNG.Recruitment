namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Models
{
    // No EP-12 fitment-data entity exists yet to source these from (EP-12 is scheduled after
    // EP-10) - HR enters them directly at generation time. This request is the fitment-data hook
    // for now.
    public class OfferLetterGenerateRequest
    {
        public long JobApplicationId { get; set; }
        public long DocumentTemplateId { get; set; }
        public string Designation { get; set; } = string.Empty;
        public decimal OfferedSalary { get; set; }
        public DateTime JoiningDate { get; set; }
        public string? ReportingManager { get; set; }
        public DateTime? OfferValidityDate { get; set; }
    }
}
