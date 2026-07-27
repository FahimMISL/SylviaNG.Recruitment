using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-09 Feature 2: one row per OTP login challenge for a Candidate. Layers on top of - does not
/// replace - Keycloak's existing RequireEmailVerification/TrySendVerifyEmailAsync link flow.
/// Only the hash/attempt/expiry audit trail lives here; the actual Keycloak refresh token
/// obtained at the initial login attempt is cached in IMemoryCache keyed by ChallengeId and is
/// never persisted to this table.
/// </summary>
public class CandidateLoginOtp : Audit
{
    public long CandidateLoginOtpId { get; set; }
    public Guid ChallengeId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string OtpCodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? ConsumedAtUtc { get; set; }
    public int AttemptCount { get; set; }
    public bool Locked { get; set; }
}
