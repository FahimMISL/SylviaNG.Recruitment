using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Local record of an HR/Admin/SuperAdmin identity (EP-15/US-111). Keycloak stays
/// the credential/identity source of truth (KeycloakUserId links back to it) - this table exists
/// so the app can list/manage role assignments without hitting the Keycloak Admin API on every read.
/// Candidates are not tracked here - they already have CandidateProfile.
/// </summary>
public class UserAccount : Audit, ICompanyScoped
{
    public long UserAccountId { get; set; }
    public string KeycloakUserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Multi-tenant: every Admin/HR account belongs to exactly one Company. Null only for
    // SuperAdmin, which is global across every company (see CompanyScopeMiddleware).
    public long? CompanyId { get; set; }

    // HRM-integration readiness: the external system's own employee identifier (nullable, unique
    // per company when set) - same idempotent-upsert anchor as Company.ExternalId, for a future
    // sync job to match an incoming employee record to this row without relying on Email (which
    // can also change on the external side).
    public string? ExternalEmployeeId { get; set; }

    public Company? Company { get; set; }
    public ICollection<UserRoleAssignment> RoleAssignments { get; set; } = new List<UserRoleAssignment>();
}
