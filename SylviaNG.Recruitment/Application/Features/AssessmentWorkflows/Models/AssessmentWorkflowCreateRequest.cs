namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models
{
    public class AssessmentWorkflowCreateRequest
    {
        public long RequisitionId { get; set; }
        public string WorkflowName { get; set; } = string.Empty;
    }
}
