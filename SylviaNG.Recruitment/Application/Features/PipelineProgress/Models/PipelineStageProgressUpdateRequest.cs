using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.PipelineProgress.Models
{
    /// <summary>
    /// Partial update for one stage card - covers start/complete/schedule/reschedule/notes
    /// (US-042 AC3) through a single flexible PATCH, same nullable-optional-field convention
    /// as JobApplicationUpdateRequest. No enforced transition graph - unlike the flat
    /// ApplicationStatusEnum lifecycle, HR can freely move a stage between statuses here.
    /// </summary>
    public class PipelineStageProgressUpdateRequest
    {
        public StageProgressStatusEnum? Status { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public string? MeetingLink { get; set; }
        public string? Notes { get; set; }
    }
}
