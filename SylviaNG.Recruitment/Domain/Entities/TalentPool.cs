using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class TalentPool : Audit
{
    public long TalentPoolId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? RankingCriteriaJson { get; set; }
    public long CreatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User CreatedByUser { get; set; } = null!;
    public ICollection<TalentPoolCandidate> TalentPoolCandidates { get; set; } = new List<TalentPoolCandidate>();
}
