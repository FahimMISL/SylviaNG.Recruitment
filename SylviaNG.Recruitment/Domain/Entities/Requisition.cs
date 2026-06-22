using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class Requisition : Audit
{
    public long RequisitionId { get; set; }
    public long? SiteId { get; set; }
    public long? DepartmentId { get; set; }
    public long? DesignationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? JobDescription { get; set; }
    public string? Justification { get; set; }
    public RequisitionTypeEnum RequisitionType { get; set; } = RequisitionTypeEnum.NewPosition;
    public RequisitionStatusEnum RequisitionStatus { get; set; } = RequisitionStatusEnum.Draft;
    public int NumberOfPositions { get; set; } = 1;
    public string? BudgetCode { get; set; }
    public string? RoleCategory { get; set; }
    public long? ReplacementEmployeeId { get; set; }
    public string? ReplacementEmployeeName { get; set; }
    public DateTime? ReplacementLastWorkingDate { get; set; }
    public long? RequestedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User? RequestedByUser { get; set; }
    public ICollection<RequisitionAttachment> Attachments { get; set; } = new List<RequisitionAttachment>();
    public ICollection<RequisitionApproval> Approvals { get; set; } = new List<RequisitionApproval>();
    public ICollection<RequisitionStageConfig> StageConfigs { get; set; } = new List<RequisitionStageConfig>();
    public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
}
