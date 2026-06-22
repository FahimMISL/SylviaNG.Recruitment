using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class AssessmentWorkflow : Audit
{
    public long AssessmentWorkflowId { get; set; }
    public long RequisitionId { get; set; }
    public string WorkflowName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Requisition Requisition { get; set; } = null!;
    public ICollection<AssessmentStage> Stages { get; set; } = new List<AssessmentStage>();
}
