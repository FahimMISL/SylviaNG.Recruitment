namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public record KeycloakTokenResult(string AccessToken, int ExpiresInSeconds, string? RefreshToken);

    public interface IKeycloakClient
    {
        /// <summary>
        /// Resource Owner Password Credentials grant against the realm token endpoint.
        /// Throws InvalidCredentialsException on invalid_grant, KeycloakUnavailableException
        /// when the server cannot be reached.
        /// </summary>
        Task<KeycloakTokenResult> TokenAsync(string username, string password);

        /// <summary>
        /// Refresh grant against the realm token endpoint - exchanges a still-valid refresh
        /// token for a new access token (and, since Keycloak rotates them by default, a new
        /// refresh token too). Throws InvalidCredentialsException when the refresh token
        /// itself has expired or been revoked, KeycloakUnavailableException when unreachable.
        /// </summary>
        Task<KeycloakTokenResult> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Creates a realm user via the Admin REST API (service-account client-credentials
        /// grant) and assigns the given realm role. Throws DuplicateException when the
        /// username/email already exists.
        /// </summary>
        Task CreateUserAsync(string username, string email, string firstName, string lastName, string password, string realmRole, bool requireEmailVerification);

        /// <summary>
        /// Looks up a realm user's internal id by username via the Admin REST API.
        /// Throws NotFoundException if no user matches.
        /// </summary>
        Task<string> GetUserIdByUsernameAsync(string username);

        /// <summary>
        /// Updates a realm user's email via the Admin REST API. Marks the address unverified
        /// (Keycloak does not re-verify automatically). Throws DuplicateException on conflict.
        /// </summary>
        Task UpdateEmailAsync(string keycloakUserId, string newEmail);

        /// <summary>
        /// Sets a realm user's password (non-temporary) via the Admin REST API.
        /// </summary>
        Task ResetPasswordAsync(string keycloakUserId, string newPassword);
    }
}
