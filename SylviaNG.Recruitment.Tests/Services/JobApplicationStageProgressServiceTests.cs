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

    // IsMandatory = false throughout: these fixtures back tests about row-creation/update
    // mechanics and auto-progression, not the mandatory-stage gate - see
    // CreatePipelineWithMandatoryStages for that.
    private static HiringPipeline CreatePipelineWithStages()
    {
        return new HiringPipeline
        {
            HiringPipelineId = 5,
            Name = "Standard Pipeline",
            Stages = new List<PipelineStage>
            {
                new() { PipelineStageId = 101, Name = "CV Screening", StageType = "CvScreening", DisplayOrder = 1, IsActive = true, IsMandatory = false },
                new() { PipelineStageId = 102, Name = "Technical Interview", StageType = "TechnicalInterview", DisplayOrder = 2, IsActive = true, IsMandatory = false },
                new() { PipelineStageId = 103, Name = "Retired Stage", StageType = "Other", DisplayOrder = 3, IsActive = false, IsMandatory = false }
            }
        };
    }

    private static HiringPipeline CreatePipelineWithAutoProgression(int? passMarks, int? autoProgressionTargetDisplayOrder)
    {
        return new HiringPipeline
        {
            HiringPipelineId = 5,
            Name = "Standard Pipeline",
            Stages = new List<PipelineStage>
            {
                new() { PipelineStageId = 101, Name = "CV Screening", StageType = "CvScreening", DisplayOrder = 1, IsActive = true, IsMandatory = false, PassMarks = passMarks, AutoProgressionTargetDisplayOrder = autoProgressionTargetDisplayOrder },
                new() { PipelineStageId = 102, Name = "Technical Interview", StageType = "TechnicalInterview", DisplayOrder = 2, IsActive = true, IsMandatory = false }
            }
        };
    }

    private static HiringPipeline CreatePipelineWithMandatoryStages()
    {
        return new HiringPipeline
        {
            HiringPipelineId = 5,
            Name = "Standard Pipeline",
            Stages = new List<PipelineStage>
            {
                new() { PipelineStageId = 101, Name = "CV Screening", StageType = "CvScreening", DisplayOrder = 1, IsActive = true, IsMandatory = true },
                new() { PipelineStageId = 102, Name = "Technical Assessment", StageType = "TechnicalAssessment", DisplayOrder = 2, IsActive = true, IsMandatory = false },
                new() { PipelineStageId = 103, Name = "Technical Interview", StageType = "TechnicalInterview", DisplayOrder = 3, IsActive = true, IsMandatory = true }
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
    public async Task UpdateStageAsync_SetStatusToInProgress_ShouldStampStageEnteredAt()
    {
        // Arrange (EP-14 US-109: "Days in Current Stage" needs a real stage-entry timestamp)
        var row = new JobApplicationStageProgress { JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.Pending };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress> { row });

        // Act
        await _service.UpdateStageAsync(1, 101, new PipelineStageProgressUpdateRequest { Status = StageProgressStatusEnum.InProgress });

        // Assert
        row.Status.Should().Be(StageProgressStatusEnum.InProgress);
        row.StageEnteredAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateStageAsync_AlreadyInProgressPatchedAgainWithInProgress_ShouldNotBumpStageEnteredAt()
    {
        // Arrange
        var originalStageEnteredAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var row = new JobApplicationStageProgress { JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.InProgress, StageEnteredAt = originalStageEnteredAt };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress> { row });

        // Act
        await _service.UpdateStageAsync(1, 101, new PipelineStageProgressUpdateRequest { Status = StageProgressStatusEnum.InProgress, Notes = "still working" });

        // Assert
        row.StageEnteredAt.Should().Be(originalStageEnteredAt);
    }

    // Auto-progression (Auto Progression Rule + Pass Marks): scoring a stage Completed
    // auto-advances the candidate into PipelineStage.AutoProgressionTargetDisplayOrder's stage
    // when Score >= PassMarks, same effect as an HR-triggered BulkAdvanceToStageAsync.

    [Fact]
    public async Task UpdateStageAsync_CompletedWithScoreMeetingPassMarksAndTargetConfigured_ShouldAutoAdvanceIntoTargetStage()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithAutoProgression(passMarks: 70, autoProgressionTargetDisplayOrder: 2));
        var row = new JobApplicationStageProgress { JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.InProgress };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress> { row });

        JobApplicationStageProgress? added = null;
        _stageProgressRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<JobApplicationStageProgress>()))
            .Callback<JobApplicationStageProgress>(p => added = p)
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateStageAsync(1, 101, new PipelineStageProgressUpdateRequest { Status = StageProgressStatusEnum.Completed, Score = 85 });

        // Assert
        row.Score.Should().Be(85);
        added.Should().NotBeNull();
        added!.PipelineStageId.Should().Be(102);
        added.Status.Should().Be(StageProgressStatusEnum.InProgress);
    }

    [Fact]
    public async Task UpdateStageAsync_CompletedWithScoreBelowPassMarks_ShouldNotAutoAdvance()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithAutoProgression(passMarks: 70, autoProgressionTargetDisplayOrder: 2));
        var row = new JobApplicationStageProgress { JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.InProgress };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress> { row });

        // Act
        await _service.UpdateStageAsync(1, 101, new PipelineStageProgressUpdateRequest { Status = StageProgressStatusEnum.Completed, Score = 50 });

        // Assert
        row.Score.Should().Be(50);
        _stageProgressRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplicationStageProgress>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStageAsync_CompletedWithScoreButNoAutoProgressionTargetConfigured_ShouldNotAutoAdvance()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithAutoProgression(passMarks: 70, autoProgressionTargetDisplayOrder: null));
        var row = new JobApplicationStageProgress { JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.InProgress };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress> { row });

        // Act
        await _service.UpdateStageAsync(1, 101, new PipelineStageProgressUpdateRequest { Status = StageProgressStatusEnum.Completed, Score = 95 });

        // Assert
        _stageProgressRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplicationStageProgress>()), Times.Never);
        // Two reads: the mandatory-order/MaxMarks gate, then the auto-progress attempt (which finds
        // no configured target and adds nothing).
        _hiringPipelineRepositoryMock.Verify(r => r.GetByIdWithStagesAsync(5), Times.Exactly(2));
    }

    [Fact]
    public async Task UpdateStageAsync_AlreadyCompletedReSavedWithScore_ShouldNotReTriggerAutoAdvance()
    {
        // Arrange: same non-bump semantics as CompletedAt/StageEnteredAt - only the transition
        // INTO Completed fires auto-progression, not every subsequent PATCH while Completed.
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        var row = new JobApplicationStageProgress { JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.Completed };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress> { row });

        // Act
        await _service.UpdateStageAsync(1, 101, new PipelineStageProgressUpdateRequest { Status = StageProgressStatusEnum.Completed, Score = 95 });

        // Assert
        row.Score.Should().Be(95);
        // Re-saving an already-Completed stage must not re-trigger auto-progression. The service now
        // reads the pipeline unconditionally (mandatory-order + MaxMarks gate), so the real proxy for
        // "did not auto-advance" is that no next-stage row was added.
        _stageProgressRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplicationStageProgress>()), Times.Never);
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

    // US-060 AC5: bulk-move all passing candidates on an exam's Results page to a HR-chosen
    // pipeline stage.

    [Fact]
    public async Task BulkAdvanceToStageAsync_NoExistingProgress_ShouldProvisionAllActiveStagesThenSetTargetInProgress()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithStages());
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress>());

        List<JobApplicationStageProgress>? addedRange = null;
        _stageProgressRepositoryMock
            .Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<JobApplicationStageProgress>>()))
            .Callback<IEnumerable<JobApplicationStageProgress>>(rows => addedRange = rows.ToList())
            .Returns(Task.CompletedTask);

        // Act
        await _service.BulkAdvanceToStageAsync(new List<long> { 1 }, 102);

        // Assert
        addedRange.Should().HaveCount(2);
        var target = addedRange!.Single(p => p.PipelineStageId == 102);
        target.Status.Should().Be(StageProgressStatusEnum.InProgress);
        target.LastUpdatedByUserName.Should().Be("abir");
        // The target row is part of this same call's fresh AddRangeAsync batch (its
        // JobApplicationStageProgressId is still the unset identity default) - calling
        // Update() on it would throw, so the property change instead rides along in the
        // pending INSERT and Update() must not be called for it.
        _stageProgressRepositoryMock.Verify(r => r.Update(It.IsAny<JobApplicationStageProgress>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task BulkAdvanceToStageAsync_ExistingProgressMissingTargetStage_ShouldAddJustThatRow()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithStages());
        var existing = new List<JobApplicationStageProgress>
        {
            new() { JobApplicationStageProgressId = 1, JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.Completed }
        };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(existing);

        JobApplicationStageProgress? added = null;
        _stageProgressRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<JobApplicationStageProgress>()))
            .Callback<JobApplicationStageProgress>(p => added = p)
            .Returns(Task.CompletedTask);

        // Act
        await _service.BulkAdvanceToStageAsync(new List<long> { 1 }, 102);

        // Assert
        added.Should().NotBeNull();
        added!.PipelineStageId.Should().Be(102);
        added.Status.Should().Be(StageProgressStatusEnum.InProgress);
        _stageProgressRepositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<JobApplicationStageProgress>>()), Times.Never);
        // Same reasoning as above: this single freshly AddAsync'd row must not also go
        // through Update().
        _stageProgressRepositoryMock.Verify(r => r.Update(It.IsAny<JobApplicationStageProgress>()), Times.Never);
    }

    [Fact]
    public async Task BulkAdvanceToStageAsync_TargetStageAlreadyHasAProgressRow_ShouldUpdateTheExistingRowNotAddANewOne()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithStages());
        var existingTargetRow = new JobApplicationStageProgress
        {
            JobApplicationStageProgressId = 2,
            JobApplicationId = 1,
            PipelineStageId = 102,
            Status = StageProgressStatusEnum.Pending,
        };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress>
        {
            new() { JobApplicationStageProgressId = 1, JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.Completed },
            existingTargetRow,
        });

        // Act
        await _service.BulkAdvanceToStageAsync(new List<long> { 1 }, 102);

        // Assert: a row that already existed before this call (a real, non-zero id) must go
        // through Update() to be marked Modified - unlike a row freshly added in this same call.
        existingTargetRow.Status.Should().Be(StageProgressStatusEnum.InProgress);
        existingTargetRow.StageEnteredAt.Should().NotBeNull();
        _stageProgressRepositoryMock.Verify(r => r.Update(existingTargetRow), Times.Once);
        _stageProgressRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplicationStageProgress>()), Times.Never);
        _stageProgressRepositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<JobApplicationStageProgress>>()), Times.Never);
    }

    [Fact]
    public async Task BulkAdvanceToStageAsync_TargetStageAlreadyInProgress_ShouldNotBumpStageEnteredAt()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithStages());
        var originalStageEnteredAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var existingTargetRow = new JobApplicationStageProgress
        {
            JobApplicationStageProgressId = 2,
            JobApplicationId = 1,
            PipelineStageId = 102,
            Status = StageProgressStatusEnum.InProgress,
            StageEnteredAt = originalStageEnteredAt,
        };
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress> { existingTargetRow });

        // Act
        await _service.BulkAdvanceToStageAsync(new List<long> { 1 }, 102);

        // Assert
        existingTargetRow.StageEnteredAt.Should().Be(originalStageEnteredAt);
    }

    [Fact]
    public async Task BulkAdvanceToStageAsync_NoHiringPipelineAssigned_ShouldThrowInvalidStatusTransitionException()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: null));

        // Act
        var act = () => _service.BulkAdvanceToStageAsync(new List<long> { 1 }, 102);

        // Assert
        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task BulkAdvanceToStageAsync_UnknownTargetStage_ShouldThrowNotFoundException()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithStages());
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress>
        {
            new() { JobApplicationStageProgressId = 1, JobApplicationId = 1, PipelineStageId = 101 }
        });

        // Act
        var act = () => _service.BulkAdvanceToStageAsync(new List<long> { 1 }, 9999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    // PipelineStage.IsMandatory gate: advancing past an incomplete mandatory stage is blocked,
    // both for a fresh application (no progress rows yet) and one with some rows already.

    [Fact]
    public async Task BulkAdvanceToStageAsync_SkippingIncompleteMandatoryStage_ShouldThrowInvalidStatusTransitionException()
    {
        // Arrange: no progress yet - CV Screening (101, mandatory) hasn't even started.
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithMandatoryStages());
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress>());
        _stageProgressRepositoryMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<JobApplicationStageProgress>>())).Returns(Task.CompletedTask);

        // Act: jump straight to Technical Interview (103), skipping mandatory CV Screening (101).
        var act = () => _service.BulkAdvanceToStageAsync(new List<long> { 1 }, 103);

        // Assert
        var ex = await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        ex.Which.Message.Should().Contain("CV Screening");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task BulkAdvanceToStageAsync_MandatoryStageAlreadyCompleted_ShouldAllowAdvance()
    {
        // Arrange
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithMandatoryStages());
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress>
        {
            new() { JobApplicationStageProgressId = 1, JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.Completed },
        });

        // Act: Technical Assessment (102) is optional, so only completed-mandatory-101 is checked.
        await _service.BulkAdvanceToStageAsync(new List<long> { 1 }, 102);

        // Assert - no throw, and the target stage advanced.
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task BulkAdvanceToStageAsync_MovingBackwardToEarlierStage_ShouldNotBeBlockedByMandatoryGate()
    {
        // Arrange: candidate is at Technical Interview (103) with CV Screening (101, mandatory)
        // completed; HR moves them back to CV Screening. The gate only checks stages before the
        // TARGET, so moving backward never has anything to check.
        SetupApplicationLookup(CreateApplication(1, hiringPipelineId: 5));
        _hiringPipelineRepositoryMock.Setup(r => r.GetByIdWithStagesAsync(5)).ReturnsAsync(CreatePipelineWithMandatoryStages());
        _stageProgressRepositoryMock.Setup(r => r.GetByJobApplicationIdAsync(1)).ReturnsAsync(new List<JobApplicationStageProgress>
        {
            new() { JobApplicationStageProgressId = 1, JobApplicationId = 1, PipelineStageId = 101, Status = StageProgressStatusEnum.Completed },
            new() { JobApplicationStageProgressId = 2, JobApplicationId = 1, PipelineStageId = 103, Status = StageProgressStatusEnum.InProgress },
        });

        // Act
        await _service.BulkAdvanceToStageAsync(new List<long> { 1 }, 101);

        // Assert - no throw.
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
