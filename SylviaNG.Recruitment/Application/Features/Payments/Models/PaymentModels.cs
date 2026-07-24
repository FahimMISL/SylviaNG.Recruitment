namespace SylviaNG.Recruitment.Application.Features.Payments.Models
{
    public class PaymentInitiateResponse
    {
        public bool Success { get; set; }
        public string? GatewayRedirectUrl { get; set; }
        public string? FailureReason { get; set; }
    }

    public class PaymentStatusResponse
    {
        public long JobApplicationId { get; set; }
        public string ApplicationStatus { get; set; } = string.Empty;
        public string? PaymentStatus { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
