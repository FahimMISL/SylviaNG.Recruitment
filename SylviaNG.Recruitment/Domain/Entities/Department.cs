using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed lookup for job vacancy department options (dynamic dropdown, same pattern as
/// Gender/BloodGroup/etc) - HR/Admin can add/rename/delete values without a code deploy.
/// </summary>
public class Department : Audit
{
    public long DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
}
