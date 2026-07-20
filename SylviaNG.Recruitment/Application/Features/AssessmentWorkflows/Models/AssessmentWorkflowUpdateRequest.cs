namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models
{
    public class AssessmentWorkflowUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        /// <summary>
        /// Full replacement of the workflow's stage list (add/edit/remove/reorder all happen
        /// here in one save, matching the drag-and-drop stage builder UX).
        /// </summary>
        public List<AssessmentStageRequest> Stages { get; set; } = new();
    }
}
