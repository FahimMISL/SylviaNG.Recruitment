using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// One row per pending email-change request, for all 3 roles (Admin/HR/Candidate) via Account
/// Settings. The OTP is sent to the NEW address (not the current one) so completing it proves
/// ownership of the inbox being switched to - only on success does the Keycloak email actually
/// change. Same hash/attempt/expiry shape as CandidateLoginOtp, but keyed by KeycloakUserId
/// rather than Username since this applies to Admin/HR too.
/// </summary>
public class EmailChangeVerification : Audit
{
    public long EmailChangeVerificationId { get; set; }
    public Guid ChallengeId { get; set; }
    public string KeycloakUserId { get; set; } = string.Empty;
    public string NewEmail { get; set; } = string.Empty;
    public string OtpCodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? ConsumedAtUtc { get; set; }
    public int AttemptCount { get; set; }
    public bool Locked { get; set; }
}
