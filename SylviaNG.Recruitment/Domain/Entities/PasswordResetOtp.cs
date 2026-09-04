using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// One row per forgot-password challenge, for all 3 roles (Admin/HR/Candidate) - closes the gap
/// where Account Settings' "change password" only works for a user who can still log in. Same
/// hash/attempt/expiry shape as EmailChangeVerification/CandidateLoginOtp, but keyed by
/// KeycloakUserId since the requester isn't authenticated yet.
/// </summary>
public class PasswordResetOtp : Audit
{
    public long PasswordResetOtpId { get; set; }
    public Guid ChallengeId { get; set; }
    public string KeycloakUserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string OtpCodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? ConsumedAtUtc { get; set; }
    public int AttemptCount { get; set; }
    public bool Locked { get; set; }
}
