using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class AssessmentWorkflowServiceTests
{
    private readonly Mock<IAssessmentWorkflowRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AssessmentWorkflowService _service;

    public AssessmentWorkflowServiceTests()
    {
        _repositoryMock = new Mock<IAssessmentWorkflowRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new AssessmentWorkflowService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static AssessmentWorkflowCreateRequest ValidCreateRequest() => new()
    {
        Name = "Standard Assessment Workflow",
        Stages = new List<AssessmentStageRequest>
        {
            new() { StageType = StageTypeEnum.WrittenTest, MaxMarks = 100, PassMarks = 40, DurationMinutes = 60, DisplayOrder = 0 },
            new() { StageType = StageTypeEnum.GroupDiscussion, MaxMarks = 50, PassMarks = 25, DurationMinutes = 30, DisplayOrder = 1 }
        }
    };

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldReturnAssessmentWorkflowIdAndPreserveStageOrder()
    {
        var request = ValidCreateRequest();

        _repositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<AssessmentWorkflow>()))
            .Callback<AssessmentWorkflow>(w => w.AssessmentWorkflowId = 1);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _service.CreateAsync(request);

        result.Should().Be(1);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<AssessmentWorkflow>(w =>
            w.Stages.Count == 2 &&
            w.Stages.ElementAt(0).DisplayOrder == 0 &&
            w.Stages.ElementAt(1).DisplayOrder == 1)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        var request = ValidCreateRequest();
        _repositoryMock.Setup(r => r.ExistsByNameAsync(request.Name, null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(request);

        await act.Should().ThrowAsync<DuplicateException>();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<AssessmentWorkflow>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReplaceStageCollectionAndRenormalizeDisplayOrder()
    {
        var entity = new AssessmentWorkflow
        {
            AssessmentWorkflowId = 1,
            Name = "Standard Assessment Workflow",
            Stages = new List<AssessmentStage>
            {
                new() { AssessmentStageId = 10, StageType = StageTypeEnum.WrittenTest, DisplayOrder = 0 }
            }
        };
        _repositoryMock.Setup(r => r.GetByIdWithStagesAsync(1)).ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.ExistsByNameAsync("Standard Assessment Workflow", 1)).ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = new AssessmentWorkflowUpdateRequest
        {
            Name = "Standard Assessment Workflow",
            Stages = new List<AssessmentStageRequest>
            {
                new() { StageType = StageTypeEnum.AptitudeTest, MaxMarks = 80, PassMarks = 30, DurationMinutes = 45, DisplayOrder = 0 },
                new() { StageType = StageTypeEnum.PsychometricTest, MaxMarks = 60, PassMarks = 20, DurationMinutes = 40, DisplayOrder = 1 }
            }
        };

        await _service.UpdateAsync(1, request);

        entity.Stages.Should().HaveCount(2);
        entity.Stages.ElementAt(0).StageType.Should().Be(StageTypeEnum.AptitudeTest);
        entity.Stages.ElementAt(0).DisplayOrder.Should().Be(0);
        entity.Stages.ElementAt(1).DisplayOrder.Should().Be(1);
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotAssignedToAnyJobPosting_ShouldDelete()
    {
        var entity = new AssessmentWorkflow { AssessmentWorkflowId = 1, Name = "Sales Assessment" };
        _repositoryMock.Setup(r => r.GetByIdWithStagesAsync(1)).ReturnsAsync(entity);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.Delete(entity), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenAssignedToJobPostings_ShouldThrowResourceInUseException()
    {
        var entity = new AssessmentWorkflow
        {
            AssessmentWorkflowId = 1,
            Name = "Sales Assessment",
            JobPostings = new List<JobPosting> { new() { JobPostingId = 10 } }
        };
        _repositoryMock.Setup(r => r.GetByIdWithStagesAsync(1)).ReturnsAsync(entity);

        var act = () => _service.DeleteAsync(1);

        await act.Should().ThrowAsync<ResourceInUseException>();
        _repositoryMock.Verify(r => r.Delete(It.IsAny<AssessmentWorkflow>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdWithStagesAsync(99)).ReturnsAsync((AssessmentWorkflow?)null);

        var act = () => _service.DeleteAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task SetActiveAsync_ShouldUpdateIsActiveFlag()
    {
        var entity = new AssessmentWorkflow { AssessmentWorkflowId = 1, Name = "Sales Assessment", IsActive = true };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.SetActiveAsync(1, false);

        entity.IsActive.Should().BeFalse();
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
    }
}
