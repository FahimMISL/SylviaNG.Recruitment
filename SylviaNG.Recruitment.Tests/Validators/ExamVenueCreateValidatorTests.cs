using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Commands.ExamVenueCreate;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;

namespace SylviaNG.Recruitment.Tests.Validators;

public class ExamVenueCreateValidatorTests
{
    private readonly ExamVenueCreateValidator _validator = new();

    private static ExamVenueCreateRequest ValidRequest() => new()
    {
        VenueName = "Main Campus",
        Location = "123 University Ave",
    };

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var command = new ExamVenueCreateCommand(ValidRequest());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyVenueName_ShouldHaveError()
    {
        var request = ValidRequest();
        request.VenueName = "";
        var command = new ExamVenueCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.VenueName");
    }

    [Fact]
    public void Validate_WithEmptyLocation_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Location = "";
        var command = new ExamVenueCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Location");
    }
}
