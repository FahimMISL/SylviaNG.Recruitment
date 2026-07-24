using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamCreate;
using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Validators;

public class ExamCreateValidatorTests
{
    private readonly ExamCreateValidator _validator = new();

    private static ExamCreateRequest ValidInPersonRequest() => new()
    {
        JobPostingId = 1,
        Title = "Written Test",
        ScheduledStartAt = DateTime.UtcNow.AddDays(3),
        DurationMinutes = 60,
        TotalMarks = 100,
        PassMarks = 40,
        ExamType = ExamTypeEnum.InPerson,
        ExamVenueId = 10,
    };

    [Fact]
    public void Validate_WithValidInPersonRequest_ShouldHaveNoErrors()
    {
        var command = new ExamCreateCommand(ValidInPersonRequest());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithValidOnlineRequest_ShouldHaveNoErrors()
    {
        var request = ValidInPersonRequest();
        request.ExamType = ExamTypeEnum.Online;
        request.ExamVenueId = null;
        request.QuestionGroupId = 5;
        var command = new ExamCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithZeroJobPostingId_ShouldHaveError()
    {
        var request = ValidInPersonRequest();
        request.JobPostingId = 0;
        var command = new ExamCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.JobPostingId");
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldHaveError()
    {
        var request = ValidInPersonRequest();
        request.Title = "";
        var command = new ExamCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Title");
    }

    [Fact]
    public void Validate_WithDefaultScheduledStartAt_ShouldHaveError()
    {
        var request = ValidInPersonRequest();
        request.ScheduledStartAt = default;
        var command = new ExamCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.ScheduledStartAt");
    }

    [Fact]
    public void Validate_WithZeroDurationMinutes_ShouldHaveError()
    {
        var request = ValidInPersonRequest();
        request.DurationMinutes = 0;
        var command = new ExamCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.DurationMinutes");
    }

    [Fact]
    public void Validate_WithZeroTotalMarks_ShouldHaveError()
    {
        var request = ValidInPersonRequest();
        request.TotalMarks = 0;
        var command = new ExamCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.TotalMarks");
    }

    [Fact]
    public void Validate_WithPassMarksGreaterThanTotalMarks_ShouldHaveError()
    {
        var request = ValidInPersonRequest();
        request.TotalMarks = 50;
        request.PassMarks = 60;
        var command = new ExamCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.PassMarks");
    }

    [Fact]
    public void Validate_WithZeroPassMarks_ShouldHaveError()
    {
        var request = ValidInPersonRequest();
        request.PassMarks = 0;
        var command = new ExamCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.PassMarks");
    }

    [Fact]
    public void Validate_InPersonWithoutExamVenueId_ShouldHaveError()
    {
        var request = ValidInPersonRequest();
        request.ExamVenueId = null;
        var command = new ExamCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.ExamVenueId");
    }

    [Fact]
    public void Validate_OnlineWithoutQuestionGroupId_ShouldHaveError()
    {
        var request = ValidInPersonRequest();
        request.ExamType = ExamTypeEnum.Online;
        request.ExamVenueId = null;
        request.QuestionGroupId = null;
        var command = new ExamCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.QuestionGroupId");
    }
}
