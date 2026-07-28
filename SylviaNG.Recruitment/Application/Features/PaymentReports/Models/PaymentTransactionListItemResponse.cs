using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.PaymentReports.Models
{
    public class PaymentTransactionListItemResponse
    {
        public long PaymentId { get; set; }
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string? CandidateEmail { get; set; }
        public string VacancyTitle { get; set; } = string.Empty;

        /// <summary>Constant "SSLCommerz" for now - this is the only gateway integrated (EP-17).</summary>
        public string PaymentMethod { get; set; } = "SSLCommerz";

        public PaymentStatusEnum PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
    }
}
