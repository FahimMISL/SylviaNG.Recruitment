namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Models
{
    public class HiringPipelineUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        /// <summary>
        /// Full replacement of the pipeline's stage list (add/edit/remove/reorder all happen
        /// here in one save, matching the drag-and-drop pipeline builder UX).
        /// </summary>
        public List<PipelineStageRequest> Stages { get; set; } = new();
    }
}
