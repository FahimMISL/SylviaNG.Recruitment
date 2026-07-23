using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// One candidate's answer to one ExamQuestion within an ExamEnrollment's online attempt
/// (US-058). SelectedOptionIds is a comma-separated list of ExamQuestionOptionId for
/// McqSingle/McqMultiple/TrueFalse; AnswerText is used for Subjective. IsCorrect/MarksAwarded
/// are null for Subjective (flagged for manual review, AC3) until HR finalizes the score via
/// the US-059 manual-score-upload path.
/// </summary>
public class ExamAnswer : Audit
{
    public long ExamAnswerId { get; set; }
    public long ExamEnrollmentId { get; set; }
    public long ExamQuestionId { get; set; }
    public string? SelectedOptionIds { get; set; }
    public string? AnswerText { get; set; }
    public bool? IsCorrect { get; set; }
    public decimal? MarksAwarded { get; set; }

    // Navigation properties
    public ExamEnrollment ExamEnrollment { get; set; } = null!;
    public ExamQuestion ExamQuestion { get; set; } = null!;
}
