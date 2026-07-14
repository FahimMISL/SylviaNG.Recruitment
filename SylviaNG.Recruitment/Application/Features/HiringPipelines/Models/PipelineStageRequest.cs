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
        public long? DepartmentId { get; set; }
        public int? EstimatedDurationMinutes { get; set; }
        public int? SlaDays { get; set; }
        public string? ColorBadge { get; set; }
        public string? EmailTemplate { get; set; }
        public bool NotifyCandidateOnEnter { get; set; } = true;
        public bool NotifyInterviewersOnAssign { get; set; } = true;
        public string? RequiredDocuments { get; set; }
        public bool AllowCandidateReschedule { get; set; }
        public string? AutoProgressionRule { get; set; }
        public bool ManualApprovalRequired { get; set; }
        public List<long> InterviewerEmployeeIds { get; set; } = new();
    }
}
