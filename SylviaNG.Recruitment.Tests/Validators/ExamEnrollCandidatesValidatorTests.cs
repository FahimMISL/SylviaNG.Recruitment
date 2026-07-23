using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamEnrollCandidates;

namespace SylviaNG.Recruitment.Tests.Validators;

public class ExamEnrollCandidatesValidatorTests
{
    private readonly ExamEnrollCandidatesValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var command = new ExamEnrollCandidatesCommand(1, new List<long> { 5, 6 });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithZeroExamId_ShouldHaveError()
    {
        var command = new ExamEnrollCandidatesCommand(0, new List<long> { 5 });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ExamId");
    }

    [Fact]
    public void Validate_WithEmptyJobApplicationIds_ShouldHaveError()
    {
        var command = new ExamEnrollCandidatesCommand(1, new List<long>());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "JobApplicationIds");
    }
}
