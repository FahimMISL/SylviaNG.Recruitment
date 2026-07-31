using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A custom, granular role (EP-15/US-112) - distinct from the 4 fixed system roles in
/// UserRoleEnum (Admin/HR/Candidate/SuperAdmin), which stay enforced the existing
/// way via [Authorize(Roles=...)] and are not rows in this table. IsSystemRole marks a mirror row
/// for each system role so they can appear alongside custom roles in the management UI, but their
/// Permissions here are informational only - the system roles' actual enforcement is unchanged.
/// </summary>
public class Role : Audit
{
    public long RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }

    public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
    public ICollection<UserRoleAssignment> UserAssignments { get; set; } = new List<UserRoleAssignment>();
}
