namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models
{
    public class HiringPipelineStageUpdateRequest
    {
        public long? JobPostingId { get; set; }
        public string? StageName { get; set; }
        public string? StageType { get; set; }
        public int? StageOrder { get; set; }
        public bool? IsMandatory { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
