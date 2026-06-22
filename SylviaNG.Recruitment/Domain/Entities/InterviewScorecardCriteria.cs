using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class InterviewScorecardCriteria : Audit
{
    public long InterviewScorecardCriteriaId { get; set; }
    public long InterviewScorecardId { get; set; }
    public string CriteriaName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Weight { get; set; } = 1;
    public int MaxScore { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public InterviewScorecard InterviewScorecard { get; set; } = null!;
}
