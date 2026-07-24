using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Read-only, seeded reference data - a district belonging to one <see cref="Division"/>.
/// The candidate address District dropdown only unlocks/populates once a Division is chosen.
/// </summary>
public class District : Audit
{
    public long DistrictId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long DivisionId { get; set; }

    public Division? Division { get; set; }
    public ICollection<Thana> Thanas { get; set; } = new List<Thana>();
}
