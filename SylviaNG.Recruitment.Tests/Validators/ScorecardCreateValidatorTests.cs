using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.Scorecards.Commands.ScorecardCreate;
using SylviaNG.Recruitment.Application.Features.Scorecards.Models;

namespace SylviaNG.Recruitment.Tests.Validators;

public class ScorecardCreateValidatorTests
{
    private readonly ScorecardCreateValidator _validator = new();

    private static ScorecardCreateRequest ValidRequest() => new()
    {
        Name = "Technical Panel Scorecard",
        Criteria = new List<ScorecardCriterionRequest>
        {
            new() { Name = "Problem Solving", Weight = 50, MaxScore = 10, DisplayOrder = 0 },
        },
    };

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var command = new ScorecardCreateCommand(ValidRequest());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Name = "";
        var command = new ScorecardCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Name");
    }

    [Fact]
    public void Validate_WithNoCriteria_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Criteria = new List<ScorecardCriterionRequest>();
        var command = new ScorecardCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Criteria");
    }

    [Fact]
    public void Validate_WithZeroWeightCriterion_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Criteria[0].Weight = 0;
        var command = new ScorecardCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
