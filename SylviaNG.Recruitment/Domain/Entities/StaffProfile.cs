using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Minimal account-settings profile for Admin/HR (non-candidate) users - just enough to hold a
/// profile photo, since Keycloak itself has no photo field. Email/password live in Keycloak
/// itself and are managed via IKeycloakClient, not mirrored here. Auto-provisioned on first
/// account-settings request, same pattern as CandidateProfile.
/// </summary>
public class StaffProfile : Audit
{
    public long StaffProfileId { get; set; }
    public string KeycloakSubjectId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePhotoPath { get; set; }
}
