using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A named, reusable weighted-criteria evaluation template (EP-08 US-066). Not tied to any one
/// interview - the same scorecard can be reused across many interviews, similar to ShortlistFilter.
/// </summary>
public class Scorecard : Audit, ICompanyScoped
{
    public long ScorecardId { get; set; }

    // Multi-tenant: the Company that owns this scorecard, stamped at creation.
    public long? CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ScorecardCriterion> Criteria { get; set; } = new List<ScorecardCriterion>();
}
