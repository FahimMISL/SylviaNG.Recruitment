using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class InterviewVenueServiceTests
{
    private readonly Mock<IInterviewVenueRepository> _interviewVenueRepositoryMock;
    private readonly Mock<IInterviewRoomRepository> _interviewRoomRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly InterviewVenueService _service;

    public InterviewVenueServiceTests()
    {
        _interviewVenueRepositoryMock = new Mock<IInterviewVenueRepository>();
        _interviewRoomRepositoryMock = new Mock<IInterviewRoomRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new InterviewVenueService(_interviewVenueRepositoryMock.Object, _interviewRoomRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static InterviewVenueCreateRequest CreateRequest(string name = "Main Campus", string location = "123 University Ave") => new()
    {
        VenueName = name,
        Location = location,
    };

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        _interviewVenueRepositoryMock.Setup(r => r.ExistsByNameAsync("Main Campus", null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(CreateRequest());

        await act.Should().ThrowAsync<DuplicateException>();
        _interviewVenueRepositoryMock.Verify(r => r.AddAsync(It.IsAny<InterviewVenue>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldSaveAndReturnId()
    {
        _interviewVenueRepositoryMock.Setup(r => r.ExistsByNameAsync("Main Campus", null)).ReturnsAsync(false);

        InterviewVenue? saved = null;
        _interviewVenueRepositoryMock.Setup(r => r.AddAsync(It.IsAny<InterviewVenue>()))
            .Callback<InterviewVenue>(v => { v.InterviewVenueId = 5; saved = v; })
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(CreateRequest());

        id.Should().Be(5);
        saved.Should().NotBeNull();
        saved!.VenueName.Should().Be("Main Campus");
        saved.IsActive.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _interviewVenueRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((InterviewVenue?)null);

        var act = () => _service.UpdateAsync(99, new InterviewVenueUpdateRequest { VenueName = "X", Location = "Y" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task SetActiveStatusAsync_ShouldToggleIsActive()
    {
        var entity = new InterviewVenue { InterviewVenueId = 1, VenueName = "Main Campus", Location = "Addr", IsActive = true };
        _interviewVenueRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        await _service.SetActiveStatusAsync(1, false);

        entity.IsActive.Should().BeFalse();
        _interviewVenueRepositoryMock.Verify(r => r.Update(entity), Times.Once);
    }
}
