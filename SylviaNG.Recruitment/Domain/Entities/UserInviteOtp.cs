using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// One row per pending staff-account invite (Admin/HR/SuperAdmin/custom role), created by
/// UserAccountService.CreateAsync. Exists because Keycloak's own execute-actions-email (its
/// native invite-link mechanism) requires Keycloak's own realm SMTP, which hits the same
/// outbound-SMTP-port block free hosts impose on this app's own email sending - so the invite
/// is delivered via this app's Brevo-backed NotificationDispatchService instead, same pattern
/// as PasswordResetOtp/CandidateLoginOtp. Keyed by Email (the invite target) rather than
/// Username since the invited user has no session yet.
/// </summary>
public class UserInviteOtp : Audit
{
    public long UserInviteOtpId { get; set; }
    public Guid ChallengeId { get; set; }
    public string KeycloakUserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string OtpCodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? ConsumedAtUtc { get; set; }
    public int AttemptCount { get; set; }
    public bool Locked { get; set; }
}
