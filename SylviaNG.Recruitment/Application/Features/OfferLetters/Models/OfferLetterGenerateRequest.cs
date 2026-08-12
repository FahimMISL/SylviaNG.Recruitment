namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Models
{
    // Still HR-typed fields, not sourced server-side from FitmentData - the frontend
    // (OfferLetterFormComponent.loadFitmentData) prefills Designation/OfferedSalary from
    // FitmentData when it exists so HR isn't retyping it, but keeps both editable since an offer
    // can legitimately differ from the fitment record.
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
