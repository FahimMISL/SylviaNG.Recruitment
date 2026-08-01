namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Models
{
    public class PipelineStageRequest
    {
        public string Name { get; set; } = string.Empty;
        public string StageType { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public string? Description { get; set; }
        public string? PassingCriteria { get; set; }
        public bool IsMandatory { get; set; } = true;
        public int? EstimatedDurationMinutes { get; set; }
        public int? SlaDays { get; set; }
        public int? MaxMarks { get; set; }
        public int? PassMarks { get; set; }
        public string? RequiredDocuments { get; set; }
        public string? AutoProgressionRule { get; set; }
        public int? AutoProgressionTargetDisplayOrder { get; set; }
        public bool ManualApprovalRequired { get; set; }
    }
}
