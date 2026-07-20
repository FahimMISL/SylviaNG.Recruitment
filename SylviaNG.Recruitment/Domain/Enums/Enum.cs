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

public enum StageTypeEnum
{
    WrittenTest,
    AptitudeTest,
    PsychometricTest,
    GroupDiscussion,
    PracticalAssessment
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
