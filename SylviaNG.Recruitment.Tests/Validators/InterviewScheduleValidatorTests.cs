using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewSchedule;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Validators;

public class InterviewScheduleValidatorTests
{
    private readonly InterviewScheduleValidator _validator = new();

    private static InterviewScheduleRequest ValidInPersonRequest() => new()
    {
        JobApplicationId = 5,
        InterviewType = InterviewTypeEnum.InPerson,
        InterviewRoomId = 20,
        ScheduledStartAt = new DateTime(2026, 8, 1, 10, 0, 0),
        ScheduledEndAt = new DateTime(2026, 8, 1, 10, 30, 0),
        Round = 1,
    };

    [Fact]
    public void Validate_WithValidInPersonRequest_ShouldHaveNoErrors()
    {
        var command = new InterviewScheduleCommand(ValidInPersonRequest());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EndAtNotAfterStartAt_ShouldHaveError()
    {
        var request = ValidInPersonRequest();
        request.ScheduledEndAt = request.ScheduledStartAt;
        var command = new InterviewScheduleCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.ScheduledEndAt");
    }

    [Fact]
    public void Validate_InPersonWithoutRoomId_ShouldHaveError()
    {
        var request = ValidInPersonRequest();
        request.InterviewRoomId = null;
        var command = new InterviewScheduleCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.InterviewRoomId");
    }

    [Fact]
    public void Validate_VirtualWithoutMeetingLink_ShouldHaveError()
    {
        var request = ValidInPersonRequest();
        request.InterviewType = InterviewTypeEnum.Virtual;
        request.InterviewRoomId = null;
        request.MeetingLink = null;
        var command = new InterviewScheduleCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.MeetingLink");
    }

    [Fact]
    public void Validate_VirtualWithMeetingLink_ShouldHaveNoErrors()
    {
        var request = ValidInPersonRequest();
        request.InterviewType = InterviewTypeEnum.Virtual;
        request.InterviewRoomId = null;
        request.MeetingLink = "https://meet.example.com/abc";
        var command = new InterviewScheduleCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
