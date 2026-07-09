namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Models
{
    public class HiringPipelineCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<PipelineStageRequest> Stages { get; set; } = new();
    }
}
