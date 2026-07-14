using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.Register;
using SylviaNG.Recruitment.Application.Features.Auth.Models;

namespace SylviaNG.Recruitment.Tests.Validators;

public class RegisterValidatorTests
{
    private readonly RegisterValidator _validator = new();

    private static RegisterRequest ValidRequest() => new()
    {
        FullName = "Kamal Hossain",
        Email = "kamal@example.com",
        Password = "secret-pass-1"
    };

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var result = _validator.Validate(new RegisterCommand(ValidRequest()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyFullName_ShouldHaveError()
    {
        var request = ValidRequest();
        request.FullName = "";

        var result = _validator.Validate(new RegisterCommand(request));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.FullName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidEmail_ShouldHaveError(string email)
    {
        var request = ValidRequest();
        request.Email = email;

        var result = _validator.Validate(new RegisterCommand(request));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Email");
    }

    [Fact]
    public void Validate_WithShortPassword_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Password = "short";

        var result = _validator.Validate(new RegisterCommand(request));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Password");
    }
}
