namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Join entity: custom Roles (EP-15/US-112) assigned to a UserAccount. A UserAccount can hold
/// several custom roles at once (multi-role assignment, US-111) on top of whatever single system
/// role Keycloak's realm_access carries.
/// </summary>
public class UserRoleAssignment
{
    public long UserAccountId { get; set; }
    public long RoleId { get; set; }

    public UserAccount UserAccount { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
