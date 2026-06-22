using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Models
{
    public class InterviewSessionUpdateRequest
    {
        public long? RequisitionId { get; set; }
        public string? SessionTitle { get; set; }
        public string? Round { get; set; }
        public InterviewModeEnum? Mode { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public int? DurationMinutes { get; set; }
        public long? InterviewVenueId { get; set; }
        public string? MeetingLink { get; set; }
        public long? ScorecardId { get; set; }
        public bool? IsActive { get; set; }
    }
}
