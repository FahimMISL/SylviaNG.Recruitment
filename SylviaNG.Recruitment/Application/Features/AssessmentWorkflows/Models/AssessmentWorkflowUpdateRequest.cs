namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models
{
    public class AssessmentWorkflowUpdateRequest
    {
        public long? RequisitionId { get; set; }
        public string? WorkflowName { get; set; }
        public bool? IsActive { get; set; }
    }
}
