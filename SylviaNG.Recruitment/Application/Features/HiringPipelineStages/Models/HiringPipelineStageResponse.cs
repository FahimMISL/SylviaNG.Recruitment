namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models
{
    public class HiringPipelineStageResponse
    {
        public long HiringPipelineStageId { get; set; }
        public long JobPostingId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public string StageType { get; set; } = string.Empty;
        public int StageOrder { get; set; }
        public bool IsMandatory { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
