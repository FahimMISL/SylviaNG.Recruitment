using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed lookup of degree titles (SSC, HSC, BSc, MBA, etc). Position is an equivalence
/// group number set by HR - degrees that are academically equivalent share the same Position
/// (e.g. Below SSC/Dakhil/O Level/SSC = 1, A Level/Alim/HSC = 2, Bachelor-level = 3, Master-level
/// = 4). The candidate Education section shows the Board dropdown only when the selected
/// Degree.Position is 1 or 2 (SSC/HSC-equivalent). No separate DegreeLevel entity - Position is
/// just an int column here, matching the company's reference Millennium HR Degree List screen.
/// </summary>
public class Degree : Audit
{
    public long DegreeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int Position { get; set; }
}
