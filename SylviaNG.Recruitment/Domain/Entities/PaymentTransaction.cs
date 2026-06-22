using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class PaymentTransaction : Audit
{
    public long PaymentTransactionId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string? SessionKey { get; set; }
    public long CandidateId { get; set; }
    public long JobPostingId { get; set; }
    public long ApplicationFeeId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BDT";
    public PaymentStatusEnum PaymentStatus { get; set; } = PaymentStatusEnum.Pending;
    public string? GatewayResponse { get; set; }
    public string? CardType { get; set; }
    public string? BankTransactionId { get; set; }
    public DateTime? PaidAt { get; set; }
    public bool IsActive { get; set; } = true;

    public Candidate? Candidate { get; set; }
    public JobPosting? JobPosting { get; set; }
    public ApplicationFee? ApplicationFee { get; set; }
}
