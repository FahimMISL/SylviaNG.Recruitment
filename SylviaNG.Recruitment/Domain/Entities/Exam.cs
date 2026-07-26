using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A scheduled exam for a set of shortlisted candidates against a job posting (US-055).
/// In-person exams require ExamVenueId; online exams require QuestionGroupId (reference only -
/// the exam-taking/auto-scoring engine is a later feature, US-058). SeatPlanGeneratedAt is the
/// only persisted "has a seat plan run" marker - seat assignments themselves live on
/// ExamEnrollment, there is no separate SeatPlan entity (US-056).
/// </summary>
public class Exam : Audit
{
    public long ExamId { get; set; }
    public long JobPostingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime ScheduledStartAt { get; set; }
    public int DurationMinutes { get; set; }
    public decimal TotalMarks { get; set; }
    public decimal PassMarks { get; set; }
    public ExamTypeEnum ExamType { get; set; }
    public long? ExamVenueId { get; set; }
    public long? QuestionGroupId { get; set; }
    public DateTime? SeatPlanGeneratedAt { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>US-058 AC6: candidate sees a submission confirmation with a reference number
    /// always; detailed score/pass-fail is only shown to them if this is true.</summary>
    public bool ShowResultsToCandidate { get; set; }

    // Navigation properties
    public JobPosting JobPosting { get; set; } = null!;
    public ExamVenue? ExamVenue { get; set; }
    public QuestionGroup? QuestionGroup { get; set; }
    public ICollection<ExamEnrollment> Enrollments { get; set; } = new List<ExamEnrollment>();
}
