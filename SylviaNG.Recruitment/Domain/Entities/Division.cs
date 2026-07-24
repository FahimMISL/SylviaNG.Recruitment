using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Read-only, seeded reference data - one of Bangladesh's 8 administrative divisions.
/// Powers the candidate address Division dropdown (top of the Division -> District -> Thana
/// cascade), same seeded-lookup shape as <see cref="SkillLibraryItem"/>.
/// </summary>
public class Division : Audit
{
    public long DivisionId { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<District> Districts { get; set; } = new List<District>();
}
