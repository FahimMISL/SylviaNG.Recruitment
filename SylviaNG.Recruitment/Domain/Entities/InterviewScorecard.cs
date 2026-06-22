using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class InterviewScorecard : Audit
{
    public long InterviewScorecardId { get; set; }
    public string ScorecardName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ScoringScale { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<InterviewScorecardCriteria> Criteria { get; set; } = new List<InterviewScorecardCriteria>();
}
