using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ApplicationScreeningResult : Audit
{
    public long ApplicationScreeningResultId { get; set; }
    public long JobApplicationId { get; set; }
    public decimal? RelevanceScore { get; set; }
    public string? MatchedKeywordsJson { get; set; }
    public string? SkillTagsJson { get; set; }
    public string? ExperienceBand { get; set; }
    public string? ScoreExplanation { get; set; }
    public DateTime ScreenedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public JobApplication JobApplication { get; set; } = null!;
}
