using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.PaymentReports.Models
{
    public class PaymentTransactionFilterRequest
    {
        public long? JobPostingId { get; set; }
        public PaymentStatusEnum? PaymentStatus { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
