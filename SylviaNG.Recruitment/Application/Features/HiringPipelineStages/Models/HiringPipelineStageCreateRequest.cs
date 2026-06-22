namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models
{
    public class HiringPipelineStageCreateRequest
    {
        public long JobPostingId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public string StageType { get; set; } = string.Empty;
        public int StageOrder { get; set; }
        public bool IsMandatory { get; set; } = true;
        public string? Description { get; set; }
    }
}
