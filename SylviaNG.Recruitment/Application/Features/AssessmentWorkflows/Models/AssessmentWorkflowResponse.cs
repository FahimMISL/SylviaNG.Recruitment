namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models
{
    public class AssessmentWorkflowResponse
    {
        public long AssessmentWorkflowId { get; set; }
        public long RequisitionId { get; set; }
        public string WorkflowName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
