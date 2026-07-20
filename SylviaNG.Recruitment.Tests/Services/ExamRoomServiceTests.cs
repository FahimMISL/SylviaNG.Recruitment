using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExamRoomServiceTests
{
    private readonly Mock<IExamRoomRepository> _examRoomRepositoryMock;
    private readonly Mock<IExamVenueRepository> _examVenueRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExamRoomService _service;

    public ExamRoomServiceTests()
    {
        _examRoomRepositoryMock = new Mock<IExamRoomRepository>();
        _examVenueRepositoryMock = new Mock<IExamVenueRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new ExamRoomService(_examRoomRepositoryMock.Object, _examVenueRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static ExamRoomCreateRequest CreateRequest(string name = "Room 101", int capacity = 30) => new()
    {
        RoomName = name,
        Capacity = capacity,
        NotifyInvigilatorsOnAssign = true,
        InvigilatorEmployeeIds = new List<long>(),
    };

    [Fact]
    public async Task CreateAsync_WithUnknownVenue_ShouldThrowNotFoundException()
    {
        _examVenueRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ExamVenue?)null);

        var act = () => _service.CreateAsync(1, CreateRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateNameInSameVenue_ShouldThrowDuplicateException()
    {
        _examVenueRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ExamVenue { ExamVenueId = 1 });
        _examRoomRepositoryMock.Setup(r => r.ExistsByNameAsync(1, "Room 101", null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(1, CreateRequest());

        await act.Should().ThrowAsync<DuplicateException>();
        _examRoomRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExamRoom>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldSaveUnderVenueAndReturnId()
    {
        _examVenueRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ExamVenue { ExamVenueId = 1 });
        _examRoomRepositoryMock.Setup(r => r.ExistsByNameAsync(1, "Room 101", null)).ReturnsAsync(false);

        ExamRoom? saved = null;
        _examRoomRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ExamRoom>()))
            .Callback<ExamRoom>(room => { room.ExamRoomId = 7; saved = room; })
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(1, CreateRequest());

        id.Should().Be(7);
        saved.Should().NotBeNull();
        saved!.ExamVenueId.Should().Be(1);
        saved.Capacity.Should().Be(30);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithUnknownInvigilatorId_ShouldThrowValidationException()
    {
        _examVenueRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ExamVenue { ExamVenueId = 1 });
        _examRoomRepositoryMock.Setup(r => r.ExistsByNameAsync(1, "Room 101", null)).ReturnsAsync(false);
        _examRoomRepositoryMock.Setup(r => r.GetExistingEmployeeIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new HashSet<long>());

        var request = CreateRequest();
        request.InvigilatorEmployeeIds = new List<long> { 42 };

        var act = () => _service.CreateAsync(1, request);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        _examRoomRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExamRoom>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReplaceInvigilatorCollectionWholesale()
    {
        var entity = new ExamRoom
        {
            ExamRoomId = 1,
            ExamVenueId = 1,
            RoomName = "Room 101",
            Capacity = 20,
            Invigilators = new List<ExamRoomInvigilator> { new() { EmployeeId = 1 } },
        };
        _examRoomRepositoryMock.Setup(r => r.GetByIdWithInvigilatorsAsync(1)).ReturnsAsync(entity);
        _examRoomRepositoryMock.Setup(r => r.ExistsByNameAsync(1, "Room 102", 1)).ReturnsAsync(false);
        _examRoomRepositoryMock.Setup(r => r.GetExistingEmployeeIdsAsync(It.IsAny<IEnumerable<long>>())).ReturnsAsync(new HashSet<long> { 2, 3 });

        var request = new ExamRoomUpdateRequest
        {
            RoomName = "Room 102",
            Capacity = 25,
            NotifyInvigilatorsOnAssign = false,
            InvigilatorEmployeeIds = new List<long> { 2, 3 },
        };

        await _service.UpdateAsync(1, request);

        entity.RoomName.Should().Be("Room 102");
        entity.Capacity.Should().Be(25);
        entity.Invigilators.Select(i => i.EmployeeId).Should().BeEquivalentTo(new long[] { 2, 3 });
    }

    [Fact]
    public async Task SetActiveStatusAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _examRoomRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ExamRoom?)null);

        var act = () => _service.SetActiveStatusAsync(99, false);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
