using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExamVenueServiceTests
{
    private readonly Mock<IExamVenueRepository> _examVenueRepositoryMock;
    private readonly Mock<IExamRoomRepository> _examRoomRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExamVenueService _service;

    public ExamVenueServiceTests()
    {
        _examVenueRepositoryMock = new Mock<IExamVenueRepository>();
        _examRoomRepositoryMock = new Mock<IExamRoomRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new ExamVenueService(_examVenueRepositoryMock.Object, _examRoomRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static ExamVenueCreateRequest CreateRequest(string name = "Main Campus") => new()
    {
        VenueName = name,
        Location = "123 University Ave",
    };

    [Fact]
    public async Task CreateAsync_WithUniqueName_ShouldSaveAndReturnId()
    {
        _examVenueRepositoryMock.Setup(r => r.ExistsByNameAsync("Main Campus", null)).ReturnsAsync(false);

        ExamVenue? saved = null;
        _examVenueRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ExamVenue>()))
            .Callback<ExamVenue>(v => { v.ExamVenueId = 5; saved = v; })
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(CreateRequest());

        id.Should().Be(5);
        saved.Should().NotBeNull();
        saved!.VenueName.Should().Be("Main Campus");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        _examVenueRepositoryMock.Setup(r => r.ExistsByNameAsync("Main Campus", null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(CreateRequest());

        await act.Should().ThrowAsync<DuplicateException>();
        _examVenueRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExamVenue>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _examVenueRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ExamVenue?)null);

        var act = () => _service.UpdateAsync(99, new ExamVenueUpdateRequest { VenueName = "X", Location = "Y" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludeRoomCountFromRoomRepository()
    {
        var entity = new ExamVenue { ExamVenueId = 1, VenueName = "Main Campus", Location = "Addr", IsActive = true };
        _examVenueRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _examRoomRepositoryMock.Setup(r => r.GetRoomCountByVenueIdAsync(1)).ReturnsAsync(3);

        var result = await _service.GetByIdAsync(1);

        result.RoomCount.Should().Be(3);
        result.VenueName.Should().Be("Main Campus");
    }

    [Fact]
    public async Task SetActiveStatusAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _examVenueRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ExamVenue?)null);

        var act = () => _service.SetActiveStatusAsync(99, false);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
