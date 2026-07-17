namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models
{
    /// <summary>Lightweight shape for the "assign workflow to job posting" dropdown.</summary>
    public class AssessmentWorkflowLookupResponse
    {
        public long AssessmentWorkflowId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
