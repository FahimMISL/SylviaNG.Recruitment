using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class QuestionGroup : Audit
{
    public long QuestionGroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? DifficultyLevel { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
