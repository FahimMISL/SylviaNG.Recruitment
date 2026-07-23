using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A scheduled interview for a job application (EP-08). Replaces the earlier unwired Interview
/// entity (single InterviewerId, no venue, no status enum, present since 2026-07-07 but never
/// referenced by any service/controller) with a schema that actually supports panel members,
/// venue/room or virtual meeting link, and an explicit Scheduled/Rescheduled/Cancelled/
/// Completed/NoShow status. PipelineStageId is a soft reference, not an FK - same convention as
/// JobApplicationStageProgress.PipelineStageId, since pipeline templates can be edited/removed
/// independently of in-flight interview schedules.
/// </summary>
public class Interview : Audit
{
    public long InterviewId { get; set; }
    public long JobApplicationId { get; set; }
    public long? PipelineStageId { get; set; }

    public InterviewTypeEnum InterviewType { get; set; }
    public long? InterviewVenueId { get; set; }
    public long? InterviewRoomId { get; set; }
    public string? MeetingLink { get; set; }

    public DateTime ScheduledStartAt { get; set; }
    public DateTime ScheduledEndAt { get; set; }

    // Feature 3 (US-070) will build real round-sequencing/gating on top of this; for now it's a
    // simple ordinal so multiple interview rounds for the same candidate can be told apart.
    public int Round { get; set; } = 1;

    public new InterviewStatusEnum Status { get; set; } = InterviewStatusEnum.Scheduled;
    public string? CancellationReason { get; set; }

    public NotificationStatusEnum EmailNotificationStatus { get; set; } = NotificationStatusEnum.Pending;
    public DateTime? EmailSentAt { get; set; }
    public string? EmailFailureReason { get; set; }

    public NotificationStatusEnum SmsNotificationStatus { get; set; } = NotificationStatusEnum.Pending;
    public DateTime? SmsLoggedAt { get; set; }

    public string? Notes { get; set; }

    // Navigation properties
    public JobApplication JobApplication { get; set; } = null!;
    public InterviewVenue? InterviewVenue { get; set; }
    public InterviewRoom? InterviewRoom { get; set; }
    public ICollection<InterviewPanelMember> PanelMembers { get; set; } = new List<InterviewPanelMember>();
}
