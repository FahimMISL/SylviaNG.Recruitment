using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.Auth.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Keycloak-backed authentication (EP-15). Login proxies the ROPC grant to Keycloak's
    /// token endpoint server-side and returns the Keycloak access token; registration
    /// creates a Candidate-role realm user via the Admin REST API (US-001).
    ///
    /// The three legacy hardcoded accounts remain ONLY as an offline fallback for when
    /// Keycloak is unreachable (shared dev server needs VPN) — same spirit as the local
    /// Docker Postgres fallback. They are never consulted when Keycloak is up.
    /// </summary>
    public class AuthService : IAuthService
    {
        private static readonly IReadOnlyList<HardcodedUser> FallbackUsers = new List<HardcodedUser>
        {
            new("admin", "admin123", UserRoleEnum.Admin, "Administrator"),
            new("abir", "abir123", UserRoleEnum.HR, "Abir"),
            new("sadia", "sadia123", UserRoleEnum.Candidate, "Sadia")
        };

        // Highest-privilege first: when a Keycloak user carries several known roles,
        // the response's single Role field reports the strongest one.
        private static readonly UserRoleEnum[] RolePriority = { UserRoleEnum.Admin, UserRoleEnum.HR, UserRoleEnum.Candidate };

        private readonly IConfiguration _configuration;
        private readonly IKeycloakClient _keycloakClient;
        private readonly KeycloakSettings _keycloakSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IConfiguration configuration,
            IKeycloakClient keycloakClient,
            IOptions<KeycloakSettings> keycloakSettings,
            ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _keycloakClient = keycloakClient;
            _keycloakSettings = keycloakSettings.Value;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var tokenResult = await _keycloakClient.TokenAsync(request.Username, request.Password);
                return BuildResponseFromKeycloakToken(tokenResult);
            }
            catch (KeycloakUnavailableException ex)
            {
                _logger.LogWarning(ex, "Keycloak unreachable — falling back to the offline hardcoded accounts.");
                return LoginWithFallbackAccounts(request);
            }
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var (firstName, lastName) = SplitFullName(request.FullName);

            await _keycloakClient.CreateUserAsync(
                username: request.Email,
                email: request.Email,
                firstName: firstName,
                lastName: lastName,
                password: request.Password,
                realmRole: UserRoleEnum.Candidate.ToString(),
                requireEmailVerification: _keycloakSettings.RequireEmailVerification);

            return new RegisterResponse
            {
                Email = request.Email,
                RequiresEmailVerification = _keycloakSettings.RequireEmailVerification
            };
        }

        private LoginResponse BuildResponseFromKeycloakToken(KeycloakTokenResult tokenResult)
        {
            // The token came straight from Keycloak over the server-side channel, so it is
            // read (not signature-validated) here just to surface user metadata; real
            // validation happens on every subsequent API call via the Keycloak JWT scheme.
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenResult.AccessToken);

            var username = jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? string.Empty;
            var displayName = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? username;
            var role = ResolveKnownRole(jwt);

            return new LoginResponse
            {
                Token = tokenResult.AccessToken,
                ExpiresAtUtc = DateTime.UtcNow.AddSeconds(tokenResult.ExpiresInSeconds),
                Username = username,
                DisplayName = displayName,
                Role = role
            };
        }

        private static string ResolveKnownRole(JwtSecurityToken jwt)
        {
            var realmAccess = jwt.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
            if (string.IsNullOrEmpty(realmAccess))
                return string.Empty;

            try
            {
                using var json = JsonDocument.Parse(realmAccess);
                if (!json.RootElement.TryGetProperty("roles", out var rolesElement))
                    return string.Empty;

                var roles = rolesElement.EnumerateArray()
                    .Select(r => r.GetString())
                    .Where(r => !string.IsNullOrEmpty(r))
                    .ToHashSet(StringComparer.Ordinal);

                var known = RolePriority.FirstOrDefault(r => roles.Contains(r.ToString()));
                return roles.Contains(known.ToString()) ? known.ToString() : string.Empty;
            }
            catch (JsonException)
            {
                return string.Empty;
            }
        }

        private static (string FirstName, string LastName) SplitFullName(string fullName)
        {
            var trimmed = fullName.Trim();
            var spaceIdx = trimmed.IndexOf(' ');
            return spaceIdx < 0
                ? (trimmed, string.Empty)
                : (trimmed[..spaceIdx], trimmed[(spaceIdx + 1)..].Trim());
        }

        // ---- Offline fallback (Keycloak unreachable) ----

        private LoginResponse LoginWithFallbackAccounts(LoginRequest request)
        {
            var user = FallbackUsers.SingleOrDefault(u =>
                string.Equals(u.Username, request.Username, StringComparison.OrdinalIgnoreCase)
                && u.Password == request.Password);

            if (user is null)
                throw new InvalidCredentialsException();

            var jwtSection = _configuration.GetSection("Jwt:Local");
            var signingKey = jwtSection.GetValue<string>("SigningKey");
            if (string.IsNullOrWhiteSpace(signingKey))
            {
                throw new InvalidOperationException(
                    "Jwt:Local:SigningKey is not configured. Set it via 'dotnet user-secrets set \"Jwt:Local:SigningKey\" \"<value>\"' for local development, or your secret store in other environments.");
            }
            var issuer = jwtSection.GetValue<string>("Issuer")
                ?? throw new ArgumentNullException("Jwt:Local:Issuer");
            var audience = jwtSection.GetValue<string>("Audience")
                ?? throw new ArgumentNullException("Jwt:Local:Audience");
            var expiryMinutes = jwtSection.GetValue<int>("ExpiryMinutes", 60);

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse
            {
                Token = tokenString,
                ExpiresAtUtc = expiresAtUtc,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Role = user.Role.ToString()
            };
        }

        private sealed record HardcodedUser(string Username, string Password, UserRoleEnum Role, string DisplayName);
    }
}
