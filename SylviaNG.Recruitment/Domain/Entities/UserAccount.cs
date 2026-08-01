using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Local record of an HR/Admin/SuperAdmin identity (EP-15/US-111). Keycloak stays
/// the credential/identity source of truth (KeycloakUserId links back to it) - this table exists
/// so the app can list/manage role assignments without hitting the Keycloak Admin API on every read.
/// Candidates are not tracked here - they already have CandidateProfile.
/// </summary>
public class UserAccount : Audit
{
    public long UserAccountId { get; set; }
    public string KeycloakUserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<UserRoleAssignment> RoleAssignments { get; set; } = new List<UserRoleAssignment>();
}
