using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace SylviaNG.Recruitment.Tests.Smoke;

/// <summary>
/// Boots the full app via <see cref="WebApplicationFactory{TEntryPoint}"/> to prove the
/// EP-03 auth wiring: the career portal is reachable with no Authorization header at all
/// (explicit [AllowAnonymous] on CareerPortalController), while the internal job board
/// requires authentication (no attribute -> inherits the global RequireAuthenticatedUser()
/// MVC filter from Program.cs).
///
/// The public career-portal assertion intentionally only checks that the request is NOT
/// rejected as unauthenticated/unauthorized (matching AuthLoginSmokeTests's own reasoning) -
/// whether the paginated query itself then succeeds depends on a reachable database, which
/// this suite should not assume. The internal-job-board 401 assertion does not depend on the
/// database at all, since the authentication filter runs before any handler/DB access.
/// </summary>
public class CareerPortalAccessSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CareerPortalAccessSmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCareerPortalJobPostings_WithNoAuthorizationHeader_ShouldNotBeRejectedAsUnauthenticated()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/recruitment/career-portal/job-postings");

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetInternalJobBoardJobPostings_WithNoAuthorizationHeader_ShouldReturn401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/recruitment/internal-job-board/job-postings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
