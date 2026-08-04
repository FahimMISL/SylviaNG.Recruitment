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

    // US-058: online attempt tracking. Null StartedAt = not started; StartedAt set + SubmittedAt
    // null = in progress; SubmittedAt set = submitted. Derived AttemptStatus is computed in
    // ExamEnrollmentMapper, not persisted.
    public DateTime? StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }

    // US-058/US-059: same fields, two possible writers - the candidate's own auto-scored
    // submission (ScoreSource=AutoScored, ScoredByUserName=null) or HR's manual upload
    // (ScoreSource=ManualUpload, ScoredByUserName=the uploading HR user, satisfying US-059
    // AC5's audit requirement - no generic AuditLog entity exists in this codebase). Username,
    // not a numeric id, matching ICurrentUserService.GetCurrentUserName()'s established
    // attribution precedent (e.g. ApplicationStatusHistory.ChangedByUserName) - Keycloak's JWT
    // here carries no parseable numeric user-id claim.
    public decimal? Score { get; set; }
    public bool? IsPassed { get; set; }
    public ScoreSourceEnum? ScoreSource { get; set; }
    public DateTime? ScoredAt { get; set; }
    public string? ScoredByUserName { get; set; }

    // Navigation properties
    public Exam Exam { get; set; } = null!;
    public JobApplication JobApplication { get; set; } = null!;
    public ExamRoom? ExamRoom { get; set; }
    public ICollection<ExamAnswer> Answers { get; set; } = new List<ExamAnswer>();
}
