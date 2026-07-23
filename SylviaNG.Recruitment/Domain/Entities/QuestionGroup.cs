using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A named grouping of exam questions (by topic or difficulty level) that questions are
/// organized under (US-053 AC2). Reusable across many exams, same "reusable, not tied to one
/// job posting" precedent as ShortlistFilter.
/// </summary>
public class QuestionGroup : Audit
{
    public long QuestionGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ExamQuestion> Questions { get; set; } = new List<ExamQuestion>();
}
