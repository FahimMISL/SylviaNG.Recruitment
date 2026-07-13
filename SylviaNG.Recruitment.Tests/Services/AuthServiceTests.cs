using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Auth.Models;
using SylviaNG.Recruitment.Application.Services;

namespace SylviaNG.Recruitment.Tests.Services;

public class AuthServiceTests
{
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

        _service = new AuthService(configuration);
    }

    [Theory]
    [InlineData("admin", "admin123", "Admin")]
    [InlineData("abir", "abir123", "HR")]
    [InlineData("sadia", "sadia123", "Candidate")]
    public async Task LoginAsync_WithValidHardcodedCredentials_ShouldReturnTokenWithCorrectRole(
        string username, string password, string expectedRole)
    {
        // Arrange
        var request = new LoginRequest { Username = username, Password = password };

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.Role.Should().Be(expectedRole);
        result.Username.Should().Be(username);
        result.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_WithUsernameInDifferentCasing_ShouldStillSucceed()
    {
        // Arrange
        var request = new LoginRequest { Username = "ADMIN", Password = "admin123" };

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task LoginAsync_WithUnknownUsername_ShouldThrowInvalidCredentialsException()
    {
        // Arrange
        var request = new LoginRequest { Username = "nobody", Password = "whatever" };

        // Act
        var act = () => _service.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ShouldThrowInvalidCredentialsException()
    {
        // Arrange
        var request = new LoginRequest { Username = "admin", Password = "wrong-password" };

        // Act
        var act = () => _service.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }
}
