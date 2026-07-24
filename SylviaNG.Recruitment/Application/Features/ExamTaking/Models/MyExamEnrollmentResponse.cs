using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ExamTaking.Models
{
    public enum ExamAttemptStatusEnum
    {
        NotStarted,
        InProgress,
        Submitted
    }

    /// <summary>One row of the current candidate's "My Exams" list (US-058 AC1), surfaced
    /// alongside their applications in the My Applications portal.</summary>
    public class MyExamEnrollmentResponse
    {
        public long ExamEnrollmentId { get; set; }
        public long JobApplicationId { get; set; }
        public long JobPostingId { get; set; }
        public string JobPostingTitle { get; set; } = string.Empty;

        public long ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public ExamTypeEnum ExamType { get; set; }
        public DateTime ScheduledStartAt { get; set; }
        public int DurationMinutes { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal PassMarks { get; set; }

        public ExamAttemptStatusEnum AttemptStatus { get; set; }
        public decimal? Score { get; set; }
        public bool? IsPassed { get; set; }

        /// <summary>True only when the exam has ShowResultsToCandidate enabled AND the
        /// candidate has submitted - gates whether Score/IsPassed should be rendered (AC6).</summary>
        public bool ResultsVisible { get; set; }
    }
}
