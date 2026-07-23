using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class HiringPipelineServiceTests
{
    private readonly Mock<IHiringPipelineRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly HiringPipelineService _service;

    public HiringPipelineServiceTests()
    {
        _repositoryMock = new Mock<IHiringPipelineRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new HiringPipelineService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static HiringPipelineCreateRequest ValidCreateRequest() => new()
    {
        Name = "Software Engineer Pipeline",
        Stages = new List<PipelineStageRequest>
        {
            new() { Name = "CV Screening", StageType = "CvScreening", DisplayOrder = 0 },
            new() { Name = "Technical Interview", StageType = "TechnicalInterview", DisplayOrder = 1 }
        }
    };

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnHiringPipelineIdAndPreserveStageOrder()
    {
        var request = ValidCreateRequest();

        _repositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<HiringPipeline>()))
            .Callback<HiringPipeline>(p => p.HiringPipelineId = 1);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _service.CreateAsync(request);

        result.Should().Be(1);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<HiringPipeline>(p =>
            p.Stages.Count == 2 &&
            p.Stages.ElementAt(0).DisplayOrder == 0 &&
            p.Stages.ElementAt(1).DisplayOrder == 1)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithAssessmentStage_ShouldPersistMaxMarksAndPassMarks()
    {
        var request = new HiringPipelineCreateRequest
        {
            Name = "Software Engineer Pipeline",
            Stages = new List<PipelineStageRequest>
            {
                new() { Name = "Written Test", StageType = "WrittenTest", DisplayOrder = 0, MaxMarks = 100, PassMarks = 40 }
            }
        };

        _repositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<HiringPipeline>()))
            .Callback<HiringPipeline>(p => p.HiringPipelineId = 1);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.CreateAsync(request);

        _repositoryMock.Verify(r => r.AddAsync(It.Is<HiringPipeline>(p =>
            p.Stages.Single().MaxMarks == 100 && p.Stages.Single().PassMarks == 40)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        var request = ValidCreateRequest();
        _repositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(request);

        await act.Should().ThrowAsync<DuplicateException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<HiringPipeline>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotAssignedToAnyJobPosting_ShouldDelete()
    {
        var entity = new HiringPipeline { HiringPipelineId = 1, Name = "Sales Pipeline" };
        _repositoryMock.Setup(r => r.GetByIdWithStagesAsync(1)).ReturnsAsync(entity);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.Delete(entity), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenAssignedToJobPostings_ShouldThrowResourceInUseException()
    {
        var entity = new HiringPipeline
        {
            HiringPipelineId = 1,
            Name = "Sales Pipeline",
            JobPostings = new List<JobPosting> { new() { JobPostingId = 10 } }
        };
        _repositoryMock.Setup(r => r.GetByIdWithStagesAsync(1)).ReturnsAsync(entity);

        var act = () => _service.DeleteAsync(1);

        await act.Should().ThrowAsync<ResourceInUseException>();
        _repositoryMock.Verify(r => r.Delete(It.IsAny<HiringPipeline>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdWithStagesAsync(99)).ReturnsAsync((HiringPipeline?)null);

        var act = () => _service.DeleteAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DuplicateAsync_WhenDefaultCopyNameTaken_ShouldAppendIncrementingSuffix()
    {
        var source = new HiringPipeline
        {
            HiringPipelineId = 1,
            Name = "Sales Pipeline",
            Stages = new List<PipelineStage>
            {
                new() { Name = "Application", StageType = "Application", DisplayOrder = 0 },
                new() { Name = "Written Test", StageType = "WrittenTest", DisplayOrder = 1, MaxMarks = 100, PassMarks = 40 }
            }
        };
        _repositoryMock.Setup(r => r.GetByIdWithStagesAsync(1)).ReturnsAsync(source);
        _repositoryMock.Setup(r => r.ExistsByNameAsync("Sales Pipeline (Copy)", null)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.ExistsByNameAsync("Sales Pipeline (Copy 2)", null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<HiringPipeline>()))
            .Callback<HiringPipeline>(p => p.HiringPipelineId = 2);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var newId = await _service.DuplicateAsync(1);

        newId.Should().Be(2);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<HiringPipeline>(p =>
            p.Name == "Sales Pipeline (Copy 2)" && p.IsActive == false && p.Stages.Count == 2)), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<HiringPipeline>(p =>
            p.Stages.Single(s => s.StageType == "WrittenTest").MaxMarks == 100 &&
            p.Stages.Single(s => s.StageType == "WrittenTest").PassMarks == 40)), Times.Once);
    }

    [Fact]
    public async Task SetActiveAsync_ShouldUpdateIsActiveFlag()
    {
        var entity = new HiringPipeline { HiringPipelineId = 1, Name = "Sales Pipeline", IsActive = true };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.SetActiveAsync(1, false);

        entity.IsActive.Should().BeFalse();
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
    }
}
