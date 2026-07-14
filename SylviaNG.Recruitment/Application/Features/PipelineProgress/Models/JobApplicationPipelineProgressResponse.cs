namespace SylviaNG.Recruitment.Application.Features.PipelineProgress.Models
{
    /// <summary>The full tracker for one application (US-042 AC1/AC5).</summary>
    public class JobApplicationPipelineProgressResponse
    {
        public long JobApplicationId { get; set; }

        /// <summary>False when the application's job posting has no HiringPipeline assigned.</summary>
        public bool HasPipeline { get; set; }
        public string? PipelineName { get; set; }
        public List<PipelineStageProgressResponse> Stages { get; set; } = new();
    }
}
