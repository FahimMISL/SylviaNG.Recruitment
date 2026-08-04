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
    }
}
