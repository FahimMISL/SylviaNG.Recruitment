using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.PipelineProgress.Models
{
    /// <summary>One stage card on the pipeline tracker (US-042 AC1/AC2).</summary>
    public class PipelineStageProgressResponse
    {
        public long PipelineStageId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public string StageType { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        // Live-joined from the current PipelineStage config (not snapshotted, unlike
        // StageName/StageType/DisplayOrder above) - null if the pipeline was edited since this
        // progress row was created and the stage no longer exists at this id. Reference info for
        // whoever's acting on this candidate, not something enforced by the system.
        public string? StageDescription { get; set; }
        public string? PassingCriteria { get; set; }
        public string? RequiredDocuments { get; set; }
        public int? EstimatedDurationMinutes { get; set; }

        public StageProgressStatusEnum Status { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public string? MeetingLink { get; set; }
        public string? Notes { get; set; }
        public decimal? Score { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? LastUpdatedByUserName { get; set; }
    }
}
