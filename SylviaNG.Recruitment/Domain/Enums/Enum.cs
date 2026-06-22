namespace SylviaNG.Recruitment.Domain.Enums;

// ── Existing Enums ──────────────────────────────────────────

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
    Withdrawn
}

// ── EPIC-01: Identity & Access ──────────────────────────────

public enum ComplaintStatusEnum
{
    Open,
    InProgress,
    Resolved,
    Closed
}

// ── EPIC-02: Career Portal & Job Posting ────────────────────

public enum PublishChannelEnum
{
    InternalBoard,
    CareerPage,
    BDJobs,
    LinkedIn
}

public enum ChannelPublishStatusEnum
{
    Pending,
    Published,
    Failed
}

public enum CareerContentTypeEnum
{
    Video,
    Testimonial,
    Banner,
    Article
}

// ── EPIC-03: Candidate Registration ─────────────────────────

public enum CandidateTypeEnum
{
    External,
    Internal
}

public enum CandidateDocumentTypeEnum
{
    CV,
    CoverLetter,
    Certificate,
    IdProof,
    Photo,
    Other
}

// ── EPIC-04: Requisition Management ─────────────────────────

public enum RequisitionStatusEnum
{
    Draft,
    Submitted,
    PendingApproval,
    Approved,
    Rejected,
    ReturnedForRevision,
    Cancelled
}

public enum RequisitionTypeEnum
{
    NewPosition,
    Replacement
}

public enum ApprovalActionEnum
{
    Approved,
    Rejected,
    ReturnedForRevision
}

// ── EPIC-05: Application & Candidate Tracking ───────────────

public enum DuplicateResolutionEnum
{
    Unresolved,
    ConfirmedDuplicate,
    FalsePositive,
    Merged
}

// ── EPIC-06: Referral Management ────────────────────────────

public enum ReferralSourceEnum
{
    Employee,
    Agency,
    Direct,
    Admin
}

public enum ReferralStatusEnum
{
    Submitted,
    InvitationSent,
    ProfileCompleted,
    Applied,
    Rejected
}

// ── EPIC-07: Shortlisting ───────────────────────────────────

public enum ShortlistStatusEnum
{
    Pending,
    Shortlisted,
    NotShortlisted,
    Confirmed
}

// ── EPIC-08: Assessment & Evaluation ────────────────────────

public enum AssessmentTypeEnum
{
    Written,
    Aptitude,
    Psychometric,
    PanelInterview,
    VideoInterview,
    ThirdParty
}

public enum QuestionTypeEnum
{
    MCQ,
    TrueFalse,
    Subjective
}

public enum ExamStatusEnum
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled
}

// ── EPIC-09: Interview Management ───────────────────────────

public enum InterviewModeEnum
{
    InPerson,
    Virtual
}

public enum InterviewResultEnum
{
    Pending,
    Passed,
    Failed,
    OnHold
}

// ── EPIC-10: Notifications ──────────────────────────────────

public enum NotificationChannelEnum
{
    Email,
    SMS,
    InApp,
    Push
}

public enum DeliveryStatusEnum
{
    Pending,
    Sent,
    Delivered,
    Failed,
    Bounced
}

public enum UserNotificationTypeEnum
{
    Info,
    Success,
    Warning,
    Error
}

// ── EPIC-11: Document & Letter Generation ───────────────────

public enum DocumentTypeEnum
{
    OfferLetter,
    AppointmentLetter,
    MedicalLetter,
    TargetLetter,
    TransferLetter,
    OfferSummary,
    JoiningInfo,
    PostJoiningInfo
}

public enum AcceptanceStatusEnum
{
    Pending,
    Accepted,
    Declined
}

// ── EPIC-12: Pre-Employment Verification ────────────────────

public enum VerificationTypeEnum
{
    DocumentVerification,
    EmploymentHistory,
    ReferenceCheck,
    NID,
    CIB,
    CMMS,
    WorldCheck,
    MedicalTest
}

public enum VerificationStatusEnum
{
    Pending,
    InProgress,
    Verified,
    Failed,
    Discrepancy
}

public enum MedicalFitnessEnum
{
    Fit,
    Unfit,
    Pending
}

// ── EPIC-13: Pre-Boarding ───────────────────────────────────

public enum PreBoardingStatusEnum
{
    Pending,
    Submitted,
    CorrectionRequested,
    Validated,
    Locked
}

// ── EPIC-15: Onboarding Integration ─────────────────────────

public enum OnboardingStageEnum
{
    PreHire,
    PostHire,
    Completed,
    Failed
}

// ── EPIC-16: Export ─────────────────────────────────────────

public enum ExportStatusEnum
{
    Queued,
    Processing,
    Completed,
    Failed
}

// ── EPIC-18: System Integrations ────────────────────────────

public enum IntegrationTypeEnum
{
    CoreHR,
    Payroll,
    BDJobs,
    LinkedIn,
    Outlook,
    HRMobileApp,
    ThirdPartyAssessment
}

// ── Eligibility Filters ────────────────────────────────────

public enum EducationLevelEnum
{
    SSC,
    HSC,
    Diploma,
    Bachelors,
    Masters,
    PhD
}

// ── Payment ────────────────────────────────────────────────

public enum PaymentStatusEnum
{
    Pending,
    Processing,
    Completed,
    Failed,
    Refunded,
    Waived
}
