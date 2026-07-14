using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A named, reusable set of minimum-bar shortlisting criteria (US-043). Not tied to any one
/// job posting - a job posting is only ever a transient parameter when previewing/applying a
/// filter, never a stored relationship, so the same filter can be reused across vacancies.
/// </summary>
public class ShortlistFilter : Audit
{
    public long ShortlistFilterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>How this filter's criteria rows combine when evaluating a candidate (AC3).</summary>
    public FilterCombinatorEnum CombineWith { get; set; } = FilterCombinatorEnum.And;

    // Navigation properties
    public ICollection<ShortlistFilterCriterion> Criteria { get; set; } = new List<ShortlistFilterCriterion>();
}
