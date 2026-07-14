using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Read-only, seeded lookup of common skill names, used to power the candidate skills
/// autocomplete (US-002 AC5). CandidateSkill.SkillLibraryItemId is nullable — a candidate can
/// still type a free-text skill that isn't in this list.
/// </summary>
public class SkillLibraryItem : Audit
{
    public long SkillLibraryItemId { get; set; }
    public string Name { get; set; } = string.Empty;
}
