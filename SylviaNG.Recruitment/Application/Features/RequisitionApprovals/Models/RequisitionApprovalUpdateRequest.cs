using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models
{
    public class RequisitionApprovalUpdateRequest
    {
        public long? RequisitionId { get; set; }
        public long? ApproverUserId { get; set; }
        public string? ApproverRole { get; set; }
        public int? ApprovalLevel { get; set; }
        public ApprovalActionEnum? Action { get; set; }
        public string? Comments { get; set; }
        public DateTime? ActionDate { get; set; }
        public bool? IsPending { get; set; }
        public bool? IsActive { get; set; }
    }
}
