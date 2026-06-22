using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class Question : Audit
{
    public long QuestionId { get; set; }
    public long QuestionGroupId { get; set; }
    public QuestionTypeEnum QuestionType { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public decimal Marks { get; set; }
    public int? TimeLimitSeconds { get; set; }
    public string? Explanation { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public QuestionGroup QuestionGroup { get; set; } = null!;
    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
}
