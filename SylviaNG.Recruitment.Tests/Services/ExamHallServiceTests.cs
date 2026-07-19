using FluentAssertions;
using FluentValidation;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExamHallServiceTests
{
    private readonly Mock<IExamHallRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExamHallService _service;

    public ExamHallServiceTests()
    {
        _repositoryMock = new Mock<IExamHallRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new ExamHallService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static ExamHallCreateRequest ValidCreateRequest() => new()
    {
        HallName = "Main Auditorium",
        Location = "123 Gulshan Avenue, Dhaka",
        TotalCapacity = 100,
        InvigilatorEmployeeIds = new List<long> { 1, 2 }
    };

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnExamHallIdAndAssignInvigilators()
    {
        var request = ValidCreateRequest();

        _repositoryMock.Setup(r => r.ExistsByNameAsync(request.HallName, null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.GetExistingEmployeeIdsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new HashSet<long> { 1, 2 });
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<ExamHall>()))
            .Callback<ExamHall>(h => h.ExamHallId = 1);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _service.CreateAsync(request);

        result.Should().Be(1);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<ExamHall>(h =>
            h.HallName == "Main Auditorium" &&
            h.Invigilators.Count == 2)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        var request = ValidCreateRequest();
        _repositoryMock.Setup(r => r.ExistsByNameAsync(request.HallName, null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(request);

        await act.Should().ThrowAsync<DuplicateException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ExamHall>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithUnknownInvigilatorEmployeeId_ShouldThrowValidationException()
    {
        var request = ValidCreateRequest();
        _repositoryMock.Setup(r => r.ExistsByNameAsync(request.HallName, null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.GetExistingEmployeeIdsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new HashSet<long> { 1 });

        var act = () => _service.CreateAsync(request);

        await act.Should().ThrowAsync<ValidationException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ExamHall>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdWithInvigilatorsAsync(99)).ReturnsAsync((ExamHall?)null);

        var act = () => _service.UpdateAsync(99, ValidCreateRequestAsUpdate());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_ShouldReplaceInvigilatorCollection()
    {
        var entity = new ExamHall
        {
            ExamHallId = 1,
            HallName = "Old Name",
            Location = "Old Location",
            TotalCapacity = 50,
            Invigilators = new List<ExamHallInvigilator> { new() { EmployeeId = 9 } }
        };
        _repositoryMock.Setup(r => r.GetByIdWithInvigilatorsAsync(1)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.ExistsByNameAsync("Main Auditorium", 1)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.GetExistingEmployeeIdsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new HashSet<long> { 1, 2 });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.UpdateAsync(1, ValidCreateRequestAsUpdate());

        entity.HallName.Should().Be("Main Auditorium");
        entity.Invigilators.Select(i => i.EmployeeId).Should().BeEquivalentTo(new long[] { 1, 2 });
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
    }

    [Fact]
    public async Task SetActiveStatusAsync_ShouldUpdateIsActiveFlag()
    {
        var entity = new ExamHall { ExamHallId = 1, HallName = "Main Auditorium", IsActive = true };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.SetActiveStatusAsync(1, false);

        entity.IsActive.Should().BeFalse();
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
    }

    private static ExamHallUpdateRequest ValidCreateRequestAsUpdate() => new()
    {
        HallName = "Main Auditorium",
        Location = "123 Gulshan Avenue, Dhaka",
        TotalCapacity = 100,
        InvigilatorEmployeeIds = new List<long> { 1, 2 }
    };
}
