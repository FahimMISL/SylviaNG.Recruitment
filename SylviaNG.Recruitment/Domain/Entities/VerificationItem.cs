using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class VerificationItem : Audit
{
    public long VerificationItemId { get; set; }
    public long VerificationWorkflowId { get; set; }
    public VerificationTypeEnum VerificationType { get; set; }
    public VerificationStatusEnum ItemStatus { get; set; } = VerificationStatusEnum.Pending;
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public string? EvidenceFileUrl { get; set; }
    public long? VerifiedByUserId { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public VerificationWorkflow VerificationWorkflow { get; set; } = null!;
    public User? VerifiedByUser { get; set; }
}
