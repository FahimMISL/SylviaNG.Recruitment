using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed lookup for candidate gender options (dynamic dropdown, replaces the old
/// BloodGroupEnum - HR/Admin can add/rename/delete values without a code deploy).
/// </summary>
public class BloodGroup : Audit
{
    public long BloodGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
}
