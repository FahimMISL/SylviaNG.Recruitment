namespace SylviaNG.Recruitment.Domain.Enums;

public enum JobStatusEnum
{
    Draft,
    Open,
    OnHold,
    Closed,
    Archived
}

public enum EmploymentTypeEnum
{
    FullTime,
    PartTime,
    Contract,
    Internship
}

public enum ApplicationStatusEnum
{
    Applied,
    Screening,
    Shortlisted,
    InterviewScheduled,
    Interviewed,
    Offered,
    Hired,
    Rejected,
    Withdrawn,

    /// <summary>EP-17: application submitted to a fee-configured vacancy, waiting on SSLCommerz
    /// payment confirmation before it becomes a real Applied record.</summary>
    AwaitingPayment,

    DuplicateDismissed
}

public enum UserRoleEnum
{
    Admin,
    HR,
    Candidate
}

public enum CircularTypeEnum
{
    InternalOnly,
    ExternalOnly,
    Both
}

public enum EducationLevelEnum
{
    BelowSSC,
    SSC,
    HSC,
    Diploma,
    Bachelor,
    Master,
    Doctorate
}

public enum ApplicationSourceEnum
{
    External,
    Internal,

    /// <summary>HR/Admin applied on the candidate's behalf (US-034), e.g. agency or direct outreach.</summary>
    Admin
}

public enum CandidateDocumentTypeEnum
{
    NID,
    EducationCertificate,
    ExperienceLetter,
    Resume,
    Other
}

public enum StageProgressStatusEnum
{
    Pending,
    InProgress,
    Completed,
    Rejected
}

public enum CriterionTypeEnum
{
    EducationLevel,
    MinExperienceYears,
    RequiredSkills,
    AgeRange,
    District,
    MinScreeningScore
}

public enum FilterCombinatorEnum
{
    And,
    Or
}

public enum HrOverrideDecisionEnum
{
    Approved,
    Rejected
}

/// <summary>EP-17: lifecycle of a single SSLCommerz payment attempt against a JobApplication.</summary>
public enum PaymentStatusEnum
{
    Pending,
    Initiated,
    Success,
    Failed,
    Cancelled
}

public enum RecommendationStatusEnum
{
    Pending,
    Accepted,
    Rejected
}

public enum QuestionTypeEnum
{
    McqSingle,
    McqMultiple,
    TrueFalse,
    Subjective
}

public enum DifficultyLevelEnum
{
    Easy,
    Medium,
    Hard
}

public enum GradingSystemEnum
{
    GPA,
    CGPA,
    Division
}

public enum ExamTypeEnum
{
    InPerson,
    Online
}

/// <summary>Delivery status for a single ExamEnrollment's email/SMS notification.
/// Skipped = channel not configured (e.g. SMTP disabled) - a valid outcome, not a failure.</summary>
public enum NotificationStatusEnum
{
    Pending,
    Sent,
    Failed,
    Skipped
}

/// <summary>How an ExamEnrollment's Score/IsPassed was set (US-058/US-059).</summary>
public enum ScoreSourceEnum
{
    AutoScored,
    ManualUpload
}

/// <summary>EP-08: whether a scheduled Interview is in-person (InterviewVenueId/InterviewRoomId) or virtual (MeetingLink).</summary>
public enum InterviewTypeEnum
{
    InPerson,
    Virtual
}

/// <summary>Lifecycle of a single scheduled Interview (EP-08). Rescheduled/Cancelled/Completed/NoShow
/// are terminal-ish states set explicitly by HR action - there is no automatic transition.</summary>
public enum InterviewStatusEnum
{
    Scheduled,
    Rescheduled,
    Cancelled,
    Completed,
    NoShow
}

/// <summary>Outcome of a Completed Interview (EP-08 US-070). Set via HR's "Mark Result" action,
/// which also transitions Status to Completed. Passed on the round with the highest Sequence for a
/// job application gates whether the next InterviewRoundConfig can be scheduled.</summary>
public enum InterviewResultEnum
{
    Pending,
    Passed,
    Failed
}

/// <summary>EP-09: recruitment lifecycle event a NotificationTemplate can be mapped to via
/// EventTemplateMapping. Extend this list as later EP-09 features need new trigger points.</summary>
public enum RecruitmentEventEnum
{
    ApplicationSubmitted,
    ApplicationWithdrawn,
    ApplicationStatusChanged,
    CandidateActionRequired,
    InterviewScheduled,
    InterviewRescheduled,
    InterviewCancelled,
    ExamEnrolled,
    AdmitCardIssued,
    ExamResultPublished,
    AccountCreatedOtp
}

/// <summary>EP-09: delivery channel a NotificationTemplate is written for. Only Email has a working
/// transport wired up so far (US-073/074) - Sms/InApp/Push are modeled now so mappings/templates for
/// them can be authored ahead of their own dispatch work landing.</summary>
public enum NotificationChannelEnum
{
    Email,
    Sms,
    InApp,
    Push
}

/// <summary>EP-09: which audience an EventTemplateMapping targets. The same RecruitmentEvent+Channel
/// commonly needs two different templates - one candidate-facing, one for internal Admin/HR - so this
/// is a mapping-key dimension, not a NotificationTemplate field.</summary>
public enum NotificationRecipientTypeEnum
{
    Candidate,
    AdminHr
}
