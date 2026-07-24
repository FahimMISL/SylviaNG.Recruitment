using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationSubmit;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;

namespace SylviaNG.Recruitment.Tests.Validators;

public class InterviewEvaluationSubmitValidatorTests
{
    private readonly InterviewEvaluationSubmitValidator _validator = new();

    private static InterviewEvaluationSubmitRequest ValidRequest() => new()
    {
        EmployeeId = 5,
        ScorecardId = 1,
        Scores = new List<InterviewEvaluationScoreRequest> { new() { ScorecardCriterionId = 1, Score = 8 } },
    };

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var command = new InterviewEvaluationSubmitCommand(1, ValidRequest());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNoEmployeeId_ShouldHaveError()
    {
        var request = ValidRequest();
        request.EmployeeId = 0;
        var command = new InterviewEvaluationSubmitCommand(1, request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.EmployeeId");
    }

    [Fact]
    public void Validate_WithNoScores_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Scores = new List<InterviewEvaluationScoreRequest>();
        var command = new InterviewEvaluationSubmitCommand(1, request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Scores");
    }
}
