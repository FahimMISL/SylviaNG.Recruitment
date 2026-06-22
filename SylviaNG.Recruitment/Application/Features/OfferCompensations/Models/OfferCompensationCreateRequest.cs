namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Models
{
    public class OfferCompensationCreateRequest
    {
        public long JobApplicationId { get; set; }
        public long? FitmentDataId { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? Frequency { get; set; }
        public bool IsWithinPermittedRange { get; set; }
        public bool RequiresAdditionalApproval { get; set; }
    }
}
