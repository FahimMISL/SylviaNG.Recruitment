namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models
{
    public class AssessmentWorkflowResponse
    {
        public long AssessmentWorkflowId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int JobPostingCount { get; set; }
        public List<AssessmentStageResponse> Stages { get; set; } = new();
    }
}
