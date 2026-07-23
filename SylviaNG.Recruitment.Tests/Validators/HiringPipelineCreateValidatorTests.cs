using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineCreate;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;

namespace SylviaNG.Recruitment.Tests.Validators;

public class HiringPipelineCreateValidatorTests
{
    private readonly HiringPipelineCreateValidator _validator = new();

    private static HiringPipelineCreateRequest ValidRequest() => new()
    {
        Name = "Software Engineer Pipeline",
        Stages = new List<PipelineStageRequest>
        {
            new() { Name = "CV Screening", StageType = "CvScreening", DisplayOrder = 0 }
        }
    };

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var command = new HiringPipelineCreateCommand(ValidRequest());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Name = "";
        var command = new HiringPipelineCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Name");
    }

    [Fact]
    public void Validate_WithNoStages_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Stages = new List<PipelineStageRequest>();
        var command = new HiringPipelineCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Stages");
    }

    [Fact]
    public void Validate_WithStageMissingName_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Stages[0].Name = "";
        var command = new HiringPipelineCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Stages[0].Name");
    }

    [Fact]
    public void Validate_WithNegativeSlaDays_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Stages[0].SlaDays = -1;
        var command = new HiringPipelineCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Stages[0].SlaDays");
    }

    [Fact]
    public void Validate_WithBothMarksNull_ShouldHaveNoError()
    {
        var request = ValidRequest();
        request.Stages[0].MaxMarks = null;
        request.Stages[0].PassMarks = null;
        var command = new HiringPipelineCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithPassMarksLessThanOrEqualToMaxMarks_ShouldHaveNoError()
    {
        var request = ValidRequest();
        request.Stages[0].MaxMarks = 100;
        request.Stages[0].PassMarks = 40;
        var command = new HiringPipelineCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithPassMarksGreaterThanMaxMarks_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Stages[0].MaxMarks = 40;
        request.Stages[0].PassMarks = 100;
        var command = new HiringPipelineCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Stages[0].PassMarks");
    }

    [Fact]
    public void Validate_WithOnlyPassMarksSet_ShouldHaveNoError()
    {
        var request = ValidRequest();
        request.Stages[0].MaxMarks = null;
        request.Stages[0].PassMarks = 40;
        var command = new HiringPipelineCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
