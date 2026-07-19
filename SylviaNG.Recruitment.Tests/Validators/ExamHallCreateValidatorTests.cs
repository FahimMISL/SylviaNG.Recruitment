using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallCreate;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;

namespace SylviaNG.Recruitment.Tests.Validators;

public class ExamHallCreateValidatorTests
{
    private readonly ExamHallCreateValidator _validator = new();

    private static ExamHallCreateRequest ValidRequest() => new()
    {
        HallName = "Main Auditorium",
        Location = "123 Gulshan Avenue, Dhaka",
        TotalCapacity = 100
    };

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var command = new ExamHallCreateCommand(ValidRequest());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyHallName_ShouldHaveError()
    {
        var request = ValidRequest();
        request.HallName = "";
        var command = new ExamHallCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.HallName");
    }

    [Fact]
    public void Validate_WithEmptyLocation_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Location = "";
        var command = new ExamHallCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Location");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_WithNonPositiveTotalCapacity_ShouldHaveError(int capacity)
    {
        var request = ValidRequest();
        request.TotalCapacity = capacity;
        var command = new ExamHallCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.TotalCapacity");
    }
}
