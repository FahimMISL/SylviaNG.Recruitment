using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Models
{
    public class InterviewResponse
    {
        public long InterviewId { get; set; }
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public long JobPostingId { get; set; }
        public long? PipelineStageId { get; set; }

        public InterviewTypeEnum InterviewType { get; set; }
        public long? InterviewVenueId { get; set; }
        public string? VenueName { get; set; }
        public long? InterviewRoomId { get; set; }
        public string? RoomName { get; set; }
        public string? MeetingLink { get; set; }

        public DateTime ScheduledStartAt { get; set; }
        public DateTime ScheduledEndAt { get; set; }
        public int Round { get; set; }

        public InterviewStatusEnum Status { get; set; }
        public string? CancellationReason { get; set; }

        public List<long> PanelistEmployeeIds { get; set; } = new();

        public NotificationStatusEnum EmailNotificationStatus { get; set; }
        public string? EmailFailureReason { get; set; }
        public NotificationStatusEnum SmsNotificationStatus { get; set; }

        public string? Notes { get; set; }
    }
}
