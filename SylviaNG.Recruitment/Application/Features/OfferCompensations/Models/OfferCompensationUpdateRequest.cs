namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Models
{
    public class OfferCompensationUpdateRequest
    {
        public long? JobApplicationId { get; set; }
        public long? FitmentDataId { get; set; }
        public string? ComponentName { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? Frequency { get; set; }
        public bool? IsWithinPermittedRange { get; set; }
        public bool? RequiresAdditionalApproval { get; set; }
        public bool? IsActive { get; set; }
    }
}
