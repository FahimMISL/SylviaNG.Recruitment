using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ExamAnswer : Audit
{
    public long ExamAnswerId { get; set; }
    public long ExamCandidateId { get; set; }
    public long QuestionId { get; set; }
    public long? SelectedOptionId { get; set; }
    public string? AnswerText { get; set; }
    public decimal? MarksAwarded { get; set; }
    public bool? IsCorrect { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ExamCandidate ExamCandidate { get; set; } = null!;
    public Question Question { get; set; } = null!;
    public QuestionOption? SelectedOption { get; set; }
}
