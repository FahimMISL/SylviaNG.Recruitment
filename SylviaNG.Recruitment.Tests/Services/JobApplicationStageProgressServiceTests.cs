using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.PipelineProgress.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using System.Linq.Expressions;

namespace SylviaNG.Recruitment.Tests.Services;

public class JobApplicationStageProgressServiceTests
{
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IHiringPipelineRepository> _hiringPipelineRepositoryMock;
    private readonly Mock<IJobApplicationStageProgressRepository> _stageProgressRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly JobApplicationStageProgressService _service;

    public JobApplicationStageProgressServiceTests()
    {
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _hiringPipelineRepositoryMock = new Mock<IHiringPipelineRepository>();
        _stageProgressRepositoryMock = new Mock<IJobApplicationStageProgressRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _currentUserServiceMock.Setup(s => s.GetCurrentUserName()).Returns("abir");
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new JobApplicationStageProgressService(
            _jobApplicationRepositoryMock.Object,
            _hiringPipelineRepositoryMock.Object,
            _stageProgressRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private static JobApplication CreateApplication(long id, long? hiringPipelineId)
    {
        return new JobApplication
        {
            JobApplicationId = id,
            JobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", HiringPipelineId = hiringPipelineId }
        };
    }

    private static HiringPipeline CreatePipelineWithStages()
    {
        return new HiringPipeline
        {
            HiringPipelineId = 5,
            Name = "Standard Pipeline",
            Stages = new List<PipelineStage>
            {
                new() { PipelineStageId = 101, Name = "CV Screening", StageType = "CvScreening", DisplayOrder = 1, IsActive = true },
                new() { PipelineStageId = 102, Name = "Technical Interview", StageType = "TechnicalInterview", DisplayOrder = 2, IsActive = true },
                new() { PipelineStageId = 103, Name = "Retired Stage", StageType = "Other", DisplayOrder = 3, IsActive = false }
            }
        };
    }

    private void SetupApplicationLookup(JobApplication application)
    {
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByIdWithIncludeAsync(
                It.IsAny<Expression<Func<JobApplication, bool>>>(),
                It.IsAny<Expression<Func<JobApplication, object>>[]>()))
            .ReturnsAsync(application);
    }

    [Fact]
    public async Task GetByJobApplicationIdAsync_JobPostingHasNoPipeline_ShouldReturnHasPipelineFalseAndEmptyStages()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: null));

        // Act
        var result = await _service.GetByJobApplicationIdAsync(1);

        // Assert
        result.HasPipeline.Should().BeFalse();
        result.Stages.Should().BeEmpty();
        _hiringPipelineRepositoryMock.Verify(r => r.GetByIdWithStagesAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task GetByJobApplicationIdAsync_UnknownJobApplicationId_ShouldThrowNotFoundException()
    {
        // Arrange
        SetupApplicationLookup(null!);

        // Act
        var act = () => _service.GetByJobApplicationIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetByJobApplicationIdAsync_FirstFetch_ShouldAutoProvisionPendingRowsForActiveStagesInDisplayOrder()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress>());
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithStages());

        List<JobApplicationStageProgress>? inserted = null;
        _stageProgressRepositoryMock
            .Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<JobApplicationStageProgress>>()))
            .Callback<IEnumerable<JobApplicationStageProgress>>(rows => inserted = rows.ToList())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.GetByJobApplicationIdAsync(1);

        // Assert: the inactive third stage is excluded, the other two are provisioned Pending, in order.
        inserted.Should().NotBeNull();
        inserted!.Should().HaveCount(2);
        inserted!.Select(s => s.PipelineStageId).Should().ContainInOrder(101L, 102L);
        inserted!.Should().OnlyContain(s => s.Status == StageProgressStatusEnum.Pending);

        result.HasPipeline.Should().BeTrue();
        result.PipelineName.Should().Be("Standard Pipeline");
        result.Stages.Should().HaveCount(2);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByJobApplicationIdAsync_SubsequentFetch_ShouldReturnExistingRowsWithoutReProvisioning()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        var existing = new List<JobApplicationStageProgress>
        {
            new() { JobApplicationId = 1, PipelineStageId = 101, StageName = "CV Screening", StageType = "CvScreening", DisplayOrder = 1, Status = StageProgressStatusEnum.Completed }
        };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(existing);
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithStages());

        // Act
        var result = await _service.GetByJobApplicationIdAsync(1);

        // Assert
        result.Stages.Should().ContainSingle();
        result.Stages.Single().Status.Should().Be(StageProgressStatusEnum.Completed);
        _stageProgressRepositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<JobApplicationStageProgress>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateStageAsync_SetStatusToCompleted_ShouldStampCompletedAt()
    {
        // Arrange
        var row = new JobApplicationStageProgress { JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.InProgress };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress> { row });

        // Act
        await _service.UpdateStageAsync(1, 101, new PipelineStageProgressUpdateRequest { Status = StageProgressStatusEnum.Completed });

        // Assert
        row.Status.Should().Be(StageProgressStatusEnum.Completed);
        row.CompletedAt.Should().NotBeNull();
        row.LastUpdatedByUserName.Should().Be("abir");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateStageAsync_AlreadyCompletedPatchedAgainWithCompleted_ShouldNotBumpCompletedAt()
    {
        // Arrange
        var originalCompletedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var row = new JobApplicationStageProgress { JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.Completed, CompletedAt = originalCompletedAt };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress> { row });

        // Act
        await _service.UpdateStageAsync(1, 101, new PipelineStageProgressUpdateRequest { Status = StageProgressStatusEnum.Completed, Notes = "still good" });

        // Assert
        row.CompletedAt.Should().Be(originalCompletedAt);
        row.Notes.Should().Be("still good");
    }

    [Fact]
    public async Task UpdateStageAsync_PartialUpdateWithOnlyNotes_ShouldLeaveStatusAndScheduleUnchanged()
    {
        // Arrange
        var scheduledDate = new DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc);
        var row = new JobApplicationStageProgress { JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.InProgress, ScheduledDate = scheduledDate };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress> { row });

        // Act
        await _service.UpdateStageAsync(1, 101, new PipelineStageProgressUpdateRequest { Notes = "waiting on candidate" });

        // Assert
        row.Status.Should().Be(StageProgressStatusEnum.InProgress);
        row.ScheduledDate.Should().Be(scheduledDate);
        row.Notes.Should().Be("waiting on candidate");
    }

    [Fact]
    public async Task UpdateStageAsync_UnknownStageRow_ShouldThrowNotFoundException()
    {
        // Arrange
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress>());

        // Act
        var act = () => _service.UpdateStageAsync(1, 999, new PipelineStageProgressUpdateRequest { Notes = "x" });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
