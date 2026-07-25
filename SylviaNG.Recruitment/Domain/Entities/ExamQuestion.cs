using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A single exam question of one of QuestionTypeEnum's types (US-053 AC1). MCQ and True/False
/// both use the Options collection (True/False is two fixed, system-seeded options) so there is
/// one "at least one correct answer" code path for every non-Subjective type; Subjective
/// questions carry an empty Options collection and rely on ModelAnswer instead.
/// </summary>
public class ExamQuestion : Audit
{
    public long ExamQuestionId { get; set; }
    public long QuestionGroupId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionTypeEnum QuestionType { get; set; }
    public DifficultyLevelEnum DifficultyLevel { get; set; }
    public decimal Marks { get; set; }
    public string? Explanation { get; set; }

    // Subjective only - the expected free-text answer, kept separate from Explanation (the
    // "why" note available to every question type).
    public string? ModelAnswer { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public QuestionGroup QuestionGroup { get; set; } = null!;
    public ICollection<ExamQuestionOption> Options { get; set; } = new List<ExamQuestionOption>();
}
