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
        /// Creates a staff account (enabled, emailVerified: false, no password set) and assigns
        /// the given realm role. Does not send any email itself - Keycloak's own
        /// execute-actions-email requires Keycloak's realm SMTP, which hits the same
        /// outbound-SMTP-port block this app's own email sending does on free hosts. The caller
        /// (UserAccountService) is responsible for delivering the invite via its own
        /// Brevo-backed NotificationDispatchService instead (see UserInviteOtp).
        /// </summary>
        Task InviteUserAsync(string username, string email, string firstName, string lastName, string realmRole);

        /// <summary>
        /// Looks up a realm user's internal id by username via the Admin REST API.
        /// Throws NotFoundException if no user matches.
        /// </summary>
        Task<string> GetUserIdByUsernameAsync(string username);

        /// <summary>
        /// Updates a realm user's email via the Admin REST API. By default marks the address
        /// unverified (Keycloak does not re-verify automatically) - pass emailVerified: true when
        /// the caller already proved ownership of the new address through its own verification
        /// step (e.g. the Account Settings email-change OTP), so Keycloak's separate email-link
        /// flow isn't also required. Throws DuplicateException on conflict.
        /// </summary>
        Task UpdateEmailAsync(string keycloakUserId, string newEmail, bool emailVerified = false);

        /// <summary>
        /// Fetches a realm user's current email via the Admin REST API. Throws NotFoundException
        /// if no such user exists.
        /// </summary>
        Task<string?> GetEmailByUserIdAsync(string keycloakUserId);

        /// <summary>
        /// Sets a realm user's password (non-temporary) via the Admin REST API.
        /// </summary>
        Task ResetPasswordAsync(string keycloakUserId, string newPassword);

        /// <summary>
        /// Creates a realm role via the Admin REST API (EP-15/US-112 custom role creation).
        /// No-ops without throwing if the role already exists, since role names are unique in
        /// Keycloak and re-creating a same-named role on retry shouldn't be an error.
        /// </summary>
        Task CreateRealmRoleAsync(string roleName);

        /// <summary>
        /// Lists all realm roles via the Admin REST API, for the "assign role" pickers in the
        /// user/role management UI (EP-15/US-111/112).
        /// </summary>
        Task<List<string>> GetRealmRolesAsync();

        /// <summary>
        /// Assigns one or more realm roles to an existing user (EP-15/US-111 multi-role assign).
        /// Additive - does not remove roles the user already has. Unlike the private single-role
        /// assignment CreateUserAsync does internally, this targets an already-existing user id.
        /// </summary>
        Task AssignRealmRolesAsync(string keycloakUserId, IEnumerable<string> realmRoles);

        /// <summary>
        /// Deletes a realm user via the Admin REST API. Best-effort/non-throwing (logs and
        /// swallows any failure) - same compensating-action shape InviteUserAsync already uses
        /// internally when its own post-creation steps fail, exposed here so callers (e.g.
        /// UserAccountService, if the local UserAccount row fails to persist after a successful
        /// Keycloak invite) can roll back an orphaned Keycloak account rather than leaving one
        /// with no matching local row and no way to be managed through the app.
        /// </summary>
        Task DeleteUserAsync(string keycloakUserId, string email);
    }
}
