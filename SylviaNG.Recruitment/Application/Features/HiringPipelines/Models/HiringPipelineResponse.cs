namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Models
{
    public class HiringPipelineResponse
    {
        public long HiringPipelineId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int JobPostingCount { get; set; }
        public List<PipelineStageResponse> Stages { get; set; } = new();
    }
}
