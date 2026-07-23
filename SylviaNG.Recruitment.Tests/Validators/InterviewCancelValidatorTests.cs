using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewCancel;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;

namespace SylviaNG.Recruitment.Tests.Validators;

public class InterviewCancelValidatorTests
{
    private readonly InterviewCancelValidator _validator = new();

    [Fact]
    public void Validate_WithReason_ShouldHaveNoErrors()
    {
        var command = new InterviewCancelCommand(1, new InterviewCancelRequest { CancellationReason = "Candidate withdrew" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyReason_ShouldHaveError()
    {
        var command = new InterviewCancelCommand(1, new InterviewCancelRequest { CancellationReason = "" });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.CancellationReason");
    }
}
