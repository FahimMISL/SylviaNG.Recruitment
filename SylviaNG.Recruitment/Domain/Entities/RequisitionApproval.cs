using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class RequisitionApproval : Audit
{
    public long RequisitionApprovalId { get; set; }
    public long RequisitionId { get; set; }
    public long ApproverUserId { get; set; }
    public string ApproverRole { get; set; } = string.Empty;
    public int ApprovalLevel { get; set; }
    public ApprovalActionEnum? Action { get; set; }
    public string? Comments { get; set; }
    public DateTime? ActionDate { get; set; }
    public bool IsPending { get; set; } = true;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Requisition Requisition { get; set; } = null!;
    public User ApproverUser { get; set; } = null!;
}
