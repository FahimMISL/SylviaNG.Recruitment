using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.Auth.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IKeycloakClient> _keycloakClientMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Local:SigningKey"] = "unit-test-signing-key-unit-test-signing-key-32b",
                ["Jwt:Local:Issuer"] = "sylviang-recruitment-local-auth-tests",
                ["Jwt:Local:Audience"] = "sylviang-recruitment-api-tests",
                ["Jwt:Local:ExpiryMinutes"] = "60"
            })
            .Build();

        _keycloakClientMock = new Mock<IKeycloakClient>();

        var settings = Options.Create(new KeycloakSettings
        {
            Authority = "http://localhost:8082/realms/sylviang",
            ClientId = "sylviang-api",
            RequireEmailVerification = true
        });

        _service = new AuthService(configuration, _keycloakClientMock.Object, settings, NullLogger<AuthService>.Instance);
    }

    private static string BuildKeycloakStyleToken(string username, string displayName, params string[] realmRoles)
    {
        var rolesJson = string.Join(",", realmRoles.Select(r => $"\"{r}\""));
        var claims = new[]
        {
            new Claim("preferred_username", username),
            new Claim("name", displayName),
            new Claim("realm_access", $"{{\"roles\":[{rolesJson}]}}", JsonClaimValueTypes.Json)
        };
        var token = new JwtSecurityToken(issuer: "http://localhost:8082/realms/sylviang", claims: claims, expires: DateTime.UtcNow.AddMinutes(5));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ---- Keycloak-backed login ----

    [Fact]
    public async Task LoginAsync_WhenKeycloakAccepts_ShouldReturnKeycloakTokenAndParsedIdentity()
    {
        var keycloakToken = BuildKeycloakStyleToken("abir", "Abir Hasan", "offline_access", "HR");
        _keycloakClientMock.Setup(k => k.TokenAsync("abir", "abir123"))
            .ReturnsAsync(new KeycloakTokenResult(keycloakToken, 300));

        var result = await _service.LoginAsync(new LoginRequest { Username = "abir", Password = "abir123" });

        result.Token.Should().Be(keycloakToken);
        result.Username.Should().Be("abir");
        result.DisplayName.Should().Be("Abir Hasan");
        result.Role.Should().Be("HR");
        result.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_WhenUserHasMultipleKnownRoles_ShouldReportHighestPrivilege()
    {
        var keycloakToken = BuildKeycloakStyleToken("root", "Root", "Candidate", "Admin", "HR");
        _keycloakClientMock.Setup(k => k.TokenAsync("root", "pw"))
            .ReturnsAsync(new KeycloakTokenResult(keycloakToken, 300));

        var result = await _service.LoginAsync(new LoginRequest { Username = "root", Password = "pw" });

        result.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task LoginAsync_WhenKeycloakRejectsCredentials_ShouldPropagateWithoutFallback()
    {
        _keycloakClientMock.Setup(k => k.TokenAsync("admin", "wrong-password"))
            .ThrowsAsync(new InvalidCredentialsException());

        // "admin" exists in the fallback list but with a different password; a Keycloak
        // rejection must NOT be rescued by the offline fallback.
        var act = () => _service.LoginAsync(new LoginRequest { Username = "admin", Password = "wrong-password" });

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    // ---- Offline fallback (Keycloak unreachable) ----

    [Theory]
    [InlineData("admin", "admin123", "Admin")]
    [InlineData("abir", "abir123", "HR")]
    [InlineData("sadia", "sadia123", "Candidate")]
    public async Task LoginAsync_WhenKeycloakUnreachable_ShouldFallBackToHardcodedAccounts(
        string username, string password, string expectedRole)
    {
        _keycloakClientMock.Setup(k => k.TokenAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new KeycloakUnavailableException("down"));

        var result = await _service.LoginAsync(new LoginRequest { Username = username, Password = password });

        result.Token.Should().NotBeNullOrWhiteSpace();
        result.Role.Should().Be(expectedRole);
        result.Username.Should().Be(username);
        result.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_WhenKeycloakUnreachableAndUnknownUser_ShouldThrowInvalidCredentialsException()
    {
        _keycloakClientMock.Setup(k => k.TokenAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new KeycloakUnavailableException("down"));

        var act = () => _service.LoginAsync(new LoginRequest { Username = "nobody", Password = "whatever" });

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    // ---- Registration ----

    [Fact]
    public async Task RegisterAsync_ShouldCreateCandidateUserWithEmailAsUsernameAndSplitName()
    {
        var request = new RegisterRequest { FullName = "Kamal Hossain Chowdhury", Email = "kamal@example.com", Password = "secret-pass-1" };

        var result = await _service.RegisterAsync(request);

        _keycloakClientMock.Verify(k => k.CreateUserAsync(
            "kamal@example.com",
            "kamal@example.com",
            "Kamal",
            "Hossain Chowdhury",
            "secret-pass-1",
            "Candidate",
            true), Times.Once);

        result.Email.Should().Be("kamal@example.com");
        result.RequiresEmailVerification.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ShouldPropagateDuplicateException()
    {
        _keycloakClientMock.Setup(k => k.CreateUserAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new DuplicateException("User", "Email", "kamal@example.com"));

        var act = () => _service.RegisterAsync(new RegisterRequest { FullName = "Kamal", Email = "kamal@example.com", Password = "secret-pass-1" });

        await act.Should().ThrowAsync<DuplicateException>();
    }
}
