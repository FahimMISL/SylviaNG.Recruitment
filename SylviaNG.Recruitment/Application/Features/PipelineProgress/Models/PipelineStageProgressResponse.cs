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
        public StageProgressStatusEnum Status { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public string? MeetingLink { get; set; }
        public string? Notes { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? LastUpdatedByUserName { get; set; }
    }
}
