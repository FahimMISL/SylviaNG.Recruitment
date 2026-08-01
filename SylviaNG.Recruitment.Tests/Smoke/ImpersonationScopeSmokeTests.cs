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
/// EP-15/US-115: /recruitment/impersonation/start is SuperAdmin-only; /end requires the calling
/// token to itself be an impersonation token (an "imp_sid" claim) - a plain Admin token has
/// neither, so both should be rejected. No SuperAdmin fallback account exists in AuthService's
/// hardcoded FallbackUsers list, so the positive "SuperAdmin can start" path is covered by live
/// verification (see Doc/features/EP-15-F3-*.md) rather than here.
/// </summary>
public class ImpersonationScopeSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string TestSigningKey = "smoke-test-only-signing-key-not-for-real-use-32b";

    private readonly WebApplicationFactory<Program> _factory;

    public ImpersonationScopeSmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Local:SigningKey"] = TestSigningKey,
                    ["CandidateLoginOtp:Enabled"] = "false",
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

    private async Task<string> LoginAsync(HttpClient client, string username, string password)
    {
        var response = await client.PostAsJsonAsync("/recruitment/auth/login", new { username, password });
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return body.RootElement.GetProperty("content").GetProperty("token").GetString()!;
    }

    [Fact]
    public async Task AdminToken_StartingImpersonation_ShouldReturn403()
    {
        // Arrange: "admin" is Admin, not SuperAdmin - the two are distinct roles.
        var client = _factory.CreateClient();
        var token = await LoginAsync(client, "admin", "admin123");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.PostAsJsonAsync("/recruitment/impersonation/start", new { targetUserAccountId = 1 });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminToken_EndingImpersonation_WithNoActiveImpersonationClaim_ShouldReturn403()
    {
        // Arrange: a normal Admin token carries no "imp_sid" claim - EndCurrentAsync should
        // reject it rather than silently no-op.
        var client = _factory.CreateClient();
        var token = await LoginAsync(client, "admin", "admin123");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.PostAsync("/recruitment/impersonation/end", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
