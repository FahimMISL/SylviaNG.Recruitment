using SylviaNG.Recruitment.Application.Features.Auth.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Exchanges a still-valid Keycloak refresh token for a new access token. Only
        /// applies to Keycloak-backed sessions - the offline fallback tokens carry no
        /// refresh token, so callers never reach this for those.
        /// </summary>
        Task<LoginResponse> RefreshAsync(string refreshToken);

        /// <summary>EP-09 Feature 2: completes a candidate login gated by LoginAsync returning
        /// RequiresOtp=true. Throws OtpVerificationException for any failure (missing/expired
        /// challenge, wrong code, locked-out) - deliberately one generic error, not distinct per
        /// case, so a caller can't tell which reason applies.</summary>
        Task<LoginResponse> VerifyOtpAsync(VerifyOtpRequest request);

        /// <summary>Re-sends a fresh code for a still-open OTP challenge, resetting its expiry and attempt count.</summary>
        Task<ResendOtpResponse> ResendOtpAsync(ResendOtpRequest request);

        /// <summary>Starts a forgot-password challenge: emails an OTP to the account's own address
        /// if the username resolves to a real Keycloak user. Always returns a ChallengeId, even for
        /// an unknown username, so this endpoint can't be used to enumerate accounts - an unresolved
        /// username just yields a challenge that will never verify.</summary>
        Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request);

        /// <summary>Completes a forgot-password challenge: verifies the OTP, then sets the new
        /// password directly via Keycloak's Admin API (no old-password check - that's the whole
        /// point of this flow). Throws OtpVerificationException for any failure, same one generic
        /// error as VerifyOtpAsync.</summary>
        Task ResetPasswordAsync(ResetPasswordRequest request);

        /// <summary>Completes a staff-account invite (see UserAccountService.CreateAsync, which
        /// creates the UserInviteOtp challenge): verifies the OTP, sets the chosen password, and
        /// marks the email verified directly via Keycloak's Admin API. Throws
        /// OtpVerificationException for any failure, same one generic error as ResetPasswordAsync.</summary>
        Task AcceptInviteAsync(AcceptInviteRequest request);
    }
}
