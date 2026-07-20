using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Commands.ExamRoomCreate;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Models;

namespace SylviaNG.Recruitment.Tests.Validators;

public class ExamRoomCreateValidatorTests
{
    private readonly ExamRoomCreateValidator _validator = new();

    private static ExamRoomCreateRequest ValidRequest() => new()
    {
        RoomName = "Room 101",
        Capacity = 30,
        NotifyInvigilatorsOnAssign = true,
        InvigilatorEmployeeIds = new List<long>(),
    };

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var command = new ExamRoomCreateCommand(1, ValidRequest());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyRoomName_ShouldHaveError()
    {
        var request = ValidRequest();
        request.RoomName = "";
        var command = new ExamRoomCreateCommand(1, request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.RoomName");
    }

    [Fact]
    public void Validate_WithZeroCapacity_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Capacity = 0;
        var command = new ExamRoomCreateCommand(1, request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Capacity");
    }
}
