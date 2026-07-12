using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SylviaNG.Recruitment.Infrastructure.Services
{
    /// <summary>
    /// Thin HTTP client over Keycloak's OpenID Connect token endpoint (login) and
    /// Admin REST API (self-registration user creation). All Keycloak traffic stays
    /// server-side so the confidential client secret never reaches the browser.
    /// </summary>
    public class KeycloakClient : IKeycloakClient
    {
        private readonly HttpClient _httpClient;
        private readonly KeycloakSettings _settings;
        private readonly ILogger<KeycloakClient> _logger;

        public KeycloakClient(HttpClient httpClient, IOptions<KeycloakSettings> settings, ILogger<KeycloakClient> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        private string TokenEndpoint => $"{_settings.Authority.TrimEnd('/')}/protocol/openid-connect/token";
        private string AdminUsersEndpoint => $"{_settings.BaseUrl}/admin/realms/{_settings.Realm}/users";
        private string AdminRolesEndpoint => $"{_settings.BaseUrl}/admin/realms/{_settings.Realm}/roles";

        public async Task<KeycloakTokenResult> TokenAsync(string username, string password)
        {
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret ?? string.Empty,
                ["username"] = username,
                ["password"] = password
            };

            var response = await PostFormAsync(TokenEndpoint, form);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // invalid_grant covers wrong credentials, disabled accounts, and
                // unverified-email ("Account is not fully set up") — all end-user errors.
                if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.BadRequest)
                {
                    _logger.LogWarning("Keycloak token request rejected for user {Username}: {Body}", username, body);
                    throw new InvalidCredentialsException();
                }

                throw new KeycloakUnavailableException($"Keycloak token endpoint returned {(int)response.StatusCode}.");
            }

            using var json = JsonDocument.Parse(body);
            var accessToken = json.RootElement.GetProperty("access_token").GetString()!;
            var expiresIn = json.RootElement.GetProperty("expires_in").GetInt32();
            return new KeycloakTokenResult(accessToken, expiresIn);
        }

        public async Task CreateUserAsync(string username, string email, string firstName, string lastName, string password, string realmRole, bool requireEmailVerification)
        {
            var adminToken = await AdminTokenAsync();

            var userPayload = new
            {
                username,
                email,
                firstName,
                lastName,
                enabled = true,
                emailVerified = !requireEmailVerification,
                requiredActions = requireEmailVerification ? new[] { "VERIFY_EMAIL" } : Array.Empty<string>(),
                credentials = new[] { new { type = "password", value = password, temporary = false } }
            };

            using var createRequest = new HttpRequestMessage(HttpMethod.Post, AdminUsersEndpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(userPayload), Encoding.UTF8, "application/json")
            };
            createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var createResponse = await SendAsync(createRequest);

            if (createResponse.StatusCode == HttpStatusCode.Conflict)
                throw new DuplicateException("User", "Email", email);

            if (!createResponse.IsSuccessStatusCode)
            {
                var body = await createResponse.Content.ReadAsStringAsync();
                _logger.LogError("Keycloak user creation failed ({Status}): {Body}", (int)createResponse.StatusCode, body);
                throw new KeycloakUnavailableException($"Keycloak user creation returned {(int)createResponse.StatusCode}.");
            }

            // New user's id comes back in the Location header.
            var userId = createResponse.Headers.Location?.Segments.Last()?.TrimEnd('/')
                ?? throw new KeycloakUnavailableException("Keycloak did not return the created user's id.");

            await AssignRealmRoleAsync(adminToken, userId, realmRole);

            if (requireEmailVerification)
            {
                // Best-effort: requires SMTP configured on the realm. The VERIFY_EMAIL
                // required action still blocks login even if this send fails.
                await TrySendVerifyEmailAsync(adminToken, userId, email);
            }
        }

        public async Task<string> GetUserIdByUsernameAsync(string username)
        {
            var adminToken = await AdminTokenAsync();

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{AdminUsersEndpoint}?username={Uri.EscapeDataString(username)}&exact=true");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            var response = await SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new KeycloakUnavailableException($"Keycloak user lookup returned {(int)response.StatusCode}.");

            var body = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(body);
            var users = json.RootElement.EnumerateArray().ToList();

            if (users.Count == 0)
                throw new NotFoundException("KeycloakUser", username);

            return users[0].GetProperty("id").GetString()!;
        }

        public async Task UpdateEmailAsync(string keycloakUserId, string newEmail)
        {
            var adminToken = await AdminTokenAsync();

            var payload = new { email = newEmail, emailVerified = false };

            using var request = new HttpRequestMessage(HttpMethod.Put, $"{AdminUsersEndpoint}/{keycloakUserId}")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            var response = await SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Conflict)
                throw new DuplicateException("User", "Email", newEmail);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Keycloak email update failed ({Status}): {Body}", (int)response.StatusCode, body);
                throw new KeycloakUnavailableException($"Keycloak email update returned {(int)response.StatusCode}.");
            }
        }

        public async Task ResetPasswordAsync(string keycloakUserId, string newPassword)
        {
            var adminToken = await AdminTokenAsync();

            var payload = new { type = "password", value = newPassword, temporary = false };

            using var request = new HttpRequestMessage(HttpMethod.Put, $"{AdminUsersEndpoint}/{keycloakUserId}/reset-password")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            var response = await SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Keycloak password reset failed ({Status}): {Body}", (int)response.StatusCode, body);
                throw new KeycloakUnavailableException($"Keycloak password reset returned {(int)response.StatusCode}.");
            }
        }

        private async Task<string> AdminTokenAsync()
        {
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret ?? string.Empty
            };

            var response = await PostFormAsync(TokenEndpoint, form);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Keycloak service-account token request failed ({Status}): {Body}", (int)response.StatusCode, body);
                throw new KeycloakUnavailableException($"Keycloak service-account token request returned {(int)response.StatusCode}.");
            }

            using var json = JsonDocument.Parse(body);
            return json.RootElement.GetProperty("access_token").GetString()!;
        }

        private async Task AssignRealmRoleAsync(string adminToken, string userId, string realmRole)
        {
            using var roleRequest = new HttpRequestMessage(HttpMethod.Get, $"{AdminRolesEndpoint}/{realmRole}");
            roleRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            var roleResponse = await SendAsync(roleRequest);

            if (!roleResponse.IsSuccessStatusCode)
                throw new KeycloakUnavailableException($"Keycloak realm role '{realmRole}' lookup returned {(int)roleResponse.StatusCode}.");

            var roleJson = await roleResponse.Content.ReadAsStringAsync();

            using var mappingRequest = new HttpRequestMessage(HttpMethod.Post, $"{AdminUsersEndpoint}/{userId}/role-mappings/realm")
            {
                Content = new StringContent($"[{roleJson}]", Encoding.UTF8, "application/json")
            };
            mappingRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            var mappingResponse = await SendAsync(mappingRequest);

            if (!mappingResponse.IsSuccessStatusCode)
                throw new KeycloakUnavailableException($"Keycloak role assignment returned {(int)mappingResponse.StatusCode}.");
        }

        private async Task TrySendVerifyEmailAsync(string adminToken, string userId, string email)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, $"{AdminUsersEndpoint}/{userId}/send-verify-email");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Keycloak send-verify-email failed for {Email} ({Status}) — realm SMTP likely not configured.", email, (int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Keycloak send-verify-email failed for {Email}.", email);
            }
        }

        private async Task<HttpResponseMessage> PostFormAsync(string url, Dictionary<string, string> form)
        {
            try
            {
                return await _httpClient.PostAsync(url, new FormUrlEncodedContent(form));
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                throw new KeycloakUnavailableException("Keycloak server is unreachable.", ex);
            }
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            try
            {
                return await _httpClient.SendAsync(request);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                throw new KeycloakUnavailableException("Keycloak server is unreachable.", ex);
            }
        }
    }
}
