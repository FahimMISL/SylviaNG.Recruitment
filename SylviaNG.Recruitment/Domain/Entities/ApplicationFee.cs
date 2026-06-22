using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ApplicationFee : Audit
{
    public long ApplicationFeeId { get; set; }
    public long JobPostingId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BDT";
    public string? PaymentMethods { get; set; }
    public string? WaiverRules { get; set; }
    public bool IsActive { get; set; } = true;

    public JobPosting? JobPosting { get; set; }
}
