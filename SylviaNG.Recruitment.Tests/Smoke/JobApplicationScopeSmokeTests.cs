using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SylviaNG.Recruitment.Application.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SylviaNG.Recruitment.Tests.Smoke;

/// <summary>
/// EP-15/US-114 gap fix: <see cref="SylviaNG.Recruitment.Controllers.JobApplicationController"/>'s
/// generic GetById/Update/Delete actions had no role restriction and no ownership check at all -
/// any authenticated user (including a Candidate) could read/tamper/delete any other candidate's
/// application by id. Now gated Admin/HR only, matching every other admin-facing action on this
/// controller. Candidates keep their own scoped access via GetMyApplications/WithdrawMyApplication
/// (already ownership-checked - see JobApplicationService.WithdrawMyApplicationAsync). Follows the
/// same WebApplicationFactory + test-only signing key pattern as AuthLoginSmokeTests.
/// </summary>
public class JobApplicationScopeSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string TestSigningKey = "smoke-test-only-signing-key-not-for-real-use-32b";

    private readonly WebApplicationFactory<Program> _factory;

    public JobApplicationScopeSmokeTests(WebApplicationFactory<Program> factory)
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

    /// <summary>Mints a "Local"-scheme token directly rather than going through a real login -
    /// there's no hardcoded Candidate account to log in as (AuthService's offline fallback was
    /// removed; a Candidate identity only exists via real Keycloak self-registration), and this
    /// test only needs to exercise the authorization check on the route, not the login flow.</summary>
    private static string MintLocalToken(string username, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "sylviang-recruitment-local-auth",
            audience: "sylviang-recruitment-api",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Theory]
    [InlineData("GET", "/recruitment/job-application/1")]
    [InlineData("PUT", "/recruitment/job-application/1")]
    [InlineData("DELETE", "/recruitment/job-application/1")]
    public async Task CandidateToken_HittingAdminOnlyJobApplicationRoute_ShouldReturn403(string method, string path)
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = MintLocalToken("test-candidate", "Candidate");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod(method), path)
        {
            Content = method == "PUT" ? JsonContent.Create(new { }) : null
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task HrToken_HittingJobApplicationGetById_ShouldNotBeRejectedAsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = await LoginAsync(client, "abir", "abir123");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/recruitment/job-application/1");

        // Assert: HR is in the allowed role list - whether the id itself resolves depends on a
        // reachable database, which this suite should not assume (same reasoning as
        // AuthLoginSmokeTests.LocalIssuedToken_ShouldBeAcceptedAsBearerToken_OnAnExistingAuthorizeEndpoint).
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }
}
