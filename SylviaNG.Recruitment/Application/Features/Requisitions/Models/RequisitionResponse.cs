using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Models
{
    public class RequisitionResponse
    {
        public long RequisitionId { get; set; }
        public long? SiteId { get; set; }
        public long? DepartmentId { get; set; }
        public long? DesignationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? JobDescription { get; set; }
        public string? Justification { get; set; }
        public RequisitionTypeEnum RequisitionType { get; set; }
        public RequisitionStatusEnum RequisitionStatus { get; set; }
        public int NumberOfPositions { get; set; }
        public string? BudgetCode { get; set; }
        public string? RoleCategory { get; set; }
        public long? ReplacementEmployeeId { get; set; }
        public string? ReplacementEmployeeName { get; set; }
        public DateTime? ReplacementLastWorkingDate { get; set; }
        public long? RequestedByUserId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
