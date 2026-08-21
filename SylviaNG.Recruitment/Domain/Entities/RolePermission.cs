using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Join entity: one row per (Module, Action) a Role is granted (EP-15/US-112 permission matrix).
/// Absence of a row means "not granted" - there is no separate IsGranted flag to keep the table
/// exactly mirroring the checked cells in the matrix grid.
/// </summary>
public class RolePermission
{
    public long RoleId { get; set; }
    public AccessControlModuleEnum Module { get; set; }
    public PermissionActionEnum Action { get; set; }

    public Role Role { get; set; } = null!;
}
