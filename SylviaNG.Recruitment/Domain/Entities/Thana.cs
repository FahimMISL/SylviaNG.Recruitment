using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Read-only, seeded reference data - a thana/upazila belonging to one <see cref="District"/>.
/// The candidate address Thana dropdown only unlocks/populates once a District is chosen.
/// </summary>
public class Thana : Audit
{
    public long ThanaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long DistrictId { get; set; }

    public District? District { get; set; }
}
