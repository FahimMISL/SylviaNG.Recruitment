using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Named bucket of candidates HR wants to revisit for future vacancies (US-039). Optionally
/// linked to the vacancy it was created for, so HR can tell at a glance which candidates are
/// for which job vacancy - the link is nullable because pools are still valid without one.
/// </summary>
public class TalentPool : Audit
{
    public long TalentPoolId { get; set; }
    public string Name { get; set; } = string.Empty;

    public long? JobPostingId { get; set; }
    public JobPosting? JobPosting { get; set; }

    public ICollection<TalentPoolCandidate> Candidates { get; set; } = new List<TalentPoolCandidate>();
}
