using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>Named bucket of candidates HR wants to revisit for future vacancies (US-039).</summary>
public class TalentPool : Audit
{
    public long TalentPoolId { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<TalentPoolCandidate> Candidates { get; set; } = new List<TalentPoolCandidate>();
}
