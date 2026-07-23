using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Models
{
    /// <summary>Schedules one Interview per candidate against a shared venue/room/panel template,
    /// staggering each candidate's slot back-to-back from StartAt by DurationMinutes + GapMinutes -
    /// same "compute then bulk-persist" shape as ExamSeatPlanService.GenerateAsync, but time-based.</summary>
    public class InterviewBulkScheduleRequest
    {
        public List<long> JobApplicationIds { get; set; } = new();
        public long? PipelineStageId { get; set; }

        public InterviewTypeEnum InterviewType { get; set; }
        public long? InterviewVenueId { get; set; }
        public long? InterviewRoomId { get; set; }
        public string? MeetingLink { get; set; }

        public DateTime StartAt { get; set; }
        public int DurationMinutes { get; set; }
        public int GapMinutes { get; set; } = 0;
        public int Round { get; set; } = 1;

        public List<long> PanelistEmployeeIds { get; set; } = new();

        public string? Notes { get; set; }
    }
}
