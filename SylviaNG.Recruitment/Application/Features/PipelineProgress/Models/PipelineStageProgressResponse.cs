using System.Text.Json.Serialization;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Application.Features.PipelineProgress.Models
{
    /// <summary>One stage card on the pipeline tracker (US-042 AC1/AC2).</summary>
    public class PipelineStageProgressResponse
    {
        public long PipelineStageId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public string StageType { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsMandatory { get; set; }

        // Live-joined from the current PipelineStage config (not snapshotted, unlike
        // StageName/StageType/DisplayOrder above) - null if the pipeline was edited since this
        // progress row was created and the stage no longer exists at this id. Reference info for
        // whoever's acting on this candidate, not something enforced by the system.
        public string? StageDescription { get; set; }
        public string? PassingCriteria { get; set; }
        public string? RequiredDocuments { get; set; }
        public int? EstimatedDurationMinutes { get; set; }

        // Score's upper bound - null means the stage was never configured as a scored assessment
        // (PipelineStage's own doc comment: only assessment-shaped stages set this), in which
        // case Score is still conceptually a percentage-style 0-100 rating.
        public int? MaxMarks { get; set; }

        public StageProgressStatusEnum Status { get; set; }
        [JsonConverter(typeof(NullableLocalDateTimeJsonConverter))]
        public DateTime? ScheduledDate { get; set; }
        public string? MeetingLink { get; set; }
        public string? Notes { get; set; }
        public decimal? Score { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? LastUpdatedByUserName { get; set; }
    }
}
