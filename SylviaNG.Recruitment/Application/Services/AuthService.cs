using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Auth.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Temporary, hardcoded-credential login used until real candidate registration
    /// and Keycloak-backed user management (EP-15/EP-16) replace it. Do not add
    /// real users here — this list is intentionally fixed to exactly three accounts.
    /// </summary>
    public class AuthService : IAuthService
    {
        private static readonly IReadOnlyList<HardcodedUser> HardcodedUsers = new List<HardcodedUser>
        {
            new("admin", "admin123", UserRoleEnum.Admin, "Administrator"),
            new("abir", "abir123", UserRoleEnum.HR, "Abir"),
            new("sadia", "sadia123", UserRoleEnum.Candidate, "Sadia")
        };

        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = HardcodedUsers.SingleOrDefault(u =>
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

            return Task.FromResult(new LoginResponse
            {
                Token = tokenString,
                ExpiresAtUtc = expiresAtUtc,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Role = user.Role.ToString()
            });
        }

        private sealed record HardcodedUser(string Username, string Password, UserRoleEnum Role, string DisplayName);
    }
}
