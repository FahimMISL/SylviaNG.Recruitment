using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.Login;
using SylviaNG.Recruitment.Application.Features.Auth.Models;

namespace SylviaNG.Recruitment.Tests.Validators;

public class LoginValidatorTests
{
    private readonly LoginValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new LoginCommand(new LoginRequest { Username = "admin", Password = "admin123" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyUsername_ShouldHaveError()
    {
        // Arrange
        var command = new LoginCommand(new LoginRequest { Username = "", Password = "admin123" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Username");
    }

    [Fact]
    public void Validate_WithEmptyPassword_ShouldHaveError()
    {
        // Arrange
        var command = new LoginCommand(new LoginRequest { Username = "admin", Password = "" });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Password");
    }
}
