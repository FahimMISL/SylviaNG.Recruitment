using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SylviaNG.Recruitment.Application.Extensions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SylviaNG.Recruitment.Tests.Smoke;

/// <summary>
/// First integration-style test in this repo (previously only unit tests existed).
/// Boots the full app via <see cref="WebApplicationFactory{TEntryPoint}"/> to prove the
/// hardcoded /recruitment/auth/login endpoint and the dual Keycloak/Local JWT scheme
/// wiring in AuthenticationExtensions actually work end-to-end, not just in isolation.
/// The Jwt:Local signing key is intentionally NOT committed in appsettings.json (see the
/// comment there), so this fixture supplies its own test-only value rather than relying
/// on a developer's local user-secrets store — keeps this suite runnable on any machine/CI.
/// Two overrides are needed for the same key: the in-memory config value (read live by
/// AuthService when it signs a token) and a PostConfigure on the "Local" JwtBearer scheme
/// (whose TokenValidationParameters are captured once at host startup, before test config
/// overrides are visible to it).
/// </summary>
public class AuthLoginSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string TestSigningKey = "smoke-test-only-signing-key-not-for-real-use-32b";

    private readonly WebApplicationFactory<Program> _factory;

    public AuthLoginSmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Local:SigningKey"] = TestSigningKey,
                });
            });

            builder.ConfigureServices(services =>
            {
                services.PostConfigure<JwtBearerOptions>(AuthenticationExtensions.LocalScheme, options =>
                {
                    options.TokenValidationParameters.IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSigningKey));
                });
            });
        });
    }

    [Theory]
    [InlineData("admin", "admin123", "Admin")]
    [InlineData("abir", "abir123", "HR")]
    [InlineData("sadia", "sadia123", "Candidate")]
    public async Task Login_WithValidHardcodedCredentials_ShouldReturn200WithToken(string username, string password, string expectedRole)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/recruitment/auth/login", new { username, password });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var content = body.RootElement.GetProperty("content");
        content.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
        content.GetProperty("role").GetString().Should().Be(expectedRole);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturn401ViaErrorEnvelope()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/recruitment/auth/login", new { username = "admin", password = "wrong" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        body.RootElement.GetProperty("hasError").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task LocalIssuedToken_ShouldBeAcceptedAsBearerToken_OnAnExistingAuthorizeEndpoint()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginResponse = await client.PostAsJsonAsync("/recruitment/auth/login", new { username = "admin", password = "admin123" });
        using var loginBody = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync());
        var token = loginBody.RootElement.GetProperty("content").GetProperty("token").GetString();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/recruitment/job-posting");

        // Assert: the point of this test is the dual-scheme wiring accepted the Local token
        // (i.e. it did NOT get rejected as unauthenticated/unauthorized). Whether the request
        // then succeeds end-to-end depends on this environment having a reachable database,
        // which a unit/smoke test suite should not assume.
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }
}
