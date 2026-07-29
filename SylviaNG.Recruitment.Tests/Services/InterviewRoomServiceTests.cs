using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class InterviewRoomServiceTests
{
    private readonly Mock<IInterviewRoomRepository> _interviewRoomRepositoryMock;
    private readonly Mock<IInterviewVenueRepository> _interviewVenueRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly InterviewRoomService _service;

    public InterviewRoomServiceTests()
    {
        _interviewRoomRepositoryMock = new Mock<IInterviewRoomRepository>();
        _interviewVenueRepositoryMock = new Mock<IInterviewVenueRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new InterviewRoomService(_interviewRoomRepositoryMock.Object, _interviewVenueRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static InterviewRoomCreateRequest CreateRequest(string name = "Room A", int capacity = 1) => new()
    {
        RoomName = name,
        Capacity = capacity,
    };

    [Fact]
    public async Task CreateAsync_WithUnknownVenue_ShouldThrowNotFoundException()
    {
        _interviewVenueRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((InterviewVenue?)null);

        var act = () => _service.CreateAsync(1, CreateRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateNameInSameVenue_ShouldThrowDuplicateException()
    {
        _interviewVenueRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new InterviewVenue { InterviewVenueId = 1 });
        _interviewRoomRepositoryMock.Setup(r => r.ExistsByNameAsync(1, "Room A", null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(1, CreateRequest());

        await act.Should().ThrowAsync<DuplicateException>();
        _interviewRoomRepositoryMock.Verify(r => r.AddAsync(It.IsAny<InterviewRoom>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldSaveUnderVenueAndReturnId()
    {
        _interviewVenueRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new InterviewVenue { InterviewVenueId = 1 });
        _interviewRoomRepositoryMock.Setup(r => r.ExistsByNameAsync(1, "Room A", null)).ReturnsAsync(false);

        InterviewRoom? saved = null;
        _interviewRoomRepositoryMock.Setup(r => r.AddAsync(It.IsAny<InterviewRoom>()))
            .Callback<InterviewRoom>(room => { room.InterviewRoomId = 7; saved = room; })
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(1, CreateRequest());

        id.Should().Be(7);
        saved.Should().NotBeNull();
        saved!.InterviewVenueId.Should().Be(1);
        saved.Capacity.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetActiveStatusAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _interviewRoomRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((InterviewRoom?)null);

        var act = () => _service.SetActiveStatusAsync(99, false);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
