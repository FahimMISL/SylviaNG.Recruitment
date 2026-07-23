using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Links a JobApplication to an Exam (US-055 AC1/AC3). Created with ExamRoomId/SeatNumber null;
/// populated later by seat-plan generation or manual reassignment (US-056) - same row, two
/// moments, no separate SeatPlan entity. SeatNumber uniqueness within an exam is an app-layer
/// check (ExamEnrollmentService), not a DB constraint - same convention as ExamRoom.RoomName.
/// </summary>
public class ExamEnrollment : Audit
{
    public long ExamEnrollmentId { get; set; }
    public long ExamId { get; set; }
    public long JobApplicationId { get; set; }

    public long? ExamRoomId { get; set; }
    public string? SeatNumber { get; set; }

    public DateTime EnrolledAt { get; set; }

    public NotificationStatusEnum EmailNotificationStatus { get; set; } = NotificationStatusEnum.Pending;
    public DateTime? EmailSentAt { get; set; }
    public string? EmailFailureReason { get; set; }

    public NotificationStatusEnum SmsNotificationStatus { get; set; } = NotificationStatusEnum.Pending;
    public DateTime? SmsLoggedAt { get; set; }

    // Navigation properties
    public Exam Exam { get; set; } = null!;
    public JobApplication JobApplication { get; set; } = null!;
    public ExamRoom? ExamRoom { get; set; }
}
