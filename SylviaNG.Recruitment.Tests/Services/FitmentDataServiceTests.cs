using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class FitmentDataServiceTests
{
    private readonly Mock<IFitmentDataRepository> _fitmentDataRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly FitmentDataService _service;

    public FitmentDataServiceTests()
    {
        _fitmentDataRepositoryMock = new Mock<IFitmentDataRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new FitmentDataService(
            _fitmentDataRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    private static FitmentDataUpsertRequest ValidRequest() => new()
    {
        JobApplicationId = 5,
        Designation = "Software Engineer",
        Grade = "G3",
        Location = "Dhaka",
        BasicSalary = 50000m,
        TotalAllowances = 10000m,
        TotalDeductions = 2000m,
    };

    [Fact]
    public async Task GetByJobApplicationIdAsync_NothingConfigured_ShouldReturnNull()
    {
        _fitmentDataRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(5)).ReturnsAsync((FitmentData?)null);

        var response = await _service.GetByJobApplicationIdAsync(5);

        response.Should().BeNull();
    }

    [Fact]
    public async Task UpsertAsync_JobApplicationDoesNotExist_ShouldThrowNotFoundException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((JobApplication?)null);

        var act = () => _service.UpsertAsync(ValidRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpsertAsync_NoExistingRow_ShouldCreateNew()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new JobApplication { JobApplicationId = 5 });
        _fitmentDataRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(5)).ReturnsAsync((FitmentData?)null);

        var response = await _service.UpsertAsync(ValidRequest());

        response.Designation.Should().Be("Software Engineer");
        response.JobApplicationId.Should().Be(5);
        _fitmentDataRepositoryMock.Verify(r => r.AddAsync(It.Is<FitmentData>(f => f.JobApplicationId == 5)), Times.Once);
    }

    [Fact]
    public async Task UpsertAsync_ExistingRow_ShouldUpdateFieldsWithoutAdding()
    {
        var existing = new FitmentData { FitmentDataId = 1, JobApplicationId = 5, Designation = "Old Title", BasicSalary = 10000m };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new JobApplication { JobApplicationId = 5 });
        _fitmentDataRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(5)).ReturnsAsync(existing);

        var response = await _service.UpsertAsync(ValidRequest());

        response.Designation.Should().Be("Software Engineer");
        response.BasicSalary.Should().Be(50000m);
        _fitmentDataRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FitmentData>()), Times.Never);
    }
}
