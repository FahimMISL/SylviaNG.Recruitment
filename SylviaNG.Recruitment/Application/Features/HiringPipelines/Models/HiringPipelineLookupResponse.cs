namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Models
{
    /// <summary>Lightweight shape for the "assign pipeline to job posting" dropdown.</summary>
    public class HiringPipelineLookupResponse
    {
        public long HiringPipelineId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
