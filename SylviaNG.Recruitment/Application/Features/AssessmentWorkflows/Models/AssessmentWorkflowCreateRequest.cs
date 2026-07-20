namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models
{
    public class AssessmentWorkflowCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<AssessmentStageRequest> Stages { get; set; } = new();
    }
}
