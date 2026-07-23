using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Models
{
    public class InterviewScheduleRequest
    {
        public long JobApplicationId { get; set; }
        public long? PipelineStageId { get; set; }

        public InterviewTypeEnum InterviewType { get; set; }
        public long? InterviewVenueId { get; set; }
        public long? InterviewRoomId { get; set; }
        public string? MeetingLink { get; set; }

        public DateTime ScheduledStartAt { get; set; }
        public DateTime ScheduledEndAt { get; set; }
        public int Round { get; set; } = 1;

        /// <summary>Optional configured round (US-070) - when set, Round is auto-derived from its
        /// Sequence and the prior round must already be Passed. Omit for postings with no
        /// configured rounds - behaves exactly as before.</summary>
        public long? InterviewRoundConfigId { get; set; }

        /// <summary>Free-typed panel member Employee IDs - no employee-lookup picker exists in
        /// this codebase yet, same precedent as PipelineStage.interviewerEmployeeIds.</summary>
        public List<long> PanelistEmployeeIds { get; set; } = new();

        public string? Notes { get; set; }
    }
}
