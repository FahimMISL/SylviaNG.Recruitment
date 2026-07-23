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
    }
}
