using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Read-only, seeded lookup of Bangladeshi universities (full name + short code), used to power
/// the candidate education University autocomplete. Same "seeded but not exhaustive" shape as
/// <see cref="SkillLibraryItem"/> - CandidateEducation.UniversityLibraryItemId is nullable, so a
/// candidate can still type a free-text institution name that isn't in this list.
/// </summary>
public class UniversityLibraryItem : Audit
{
    public long UniversityLibraryItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
