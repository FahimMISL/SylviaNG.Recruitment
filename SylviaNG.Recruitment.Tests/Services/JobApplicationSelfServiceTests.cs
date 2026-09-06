using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Features.Payments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Domain.Events;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Tests.Services;

public class JobApplicationSelfServiceTests
{
    private readonly Mock<ICurrentCandidateService> _currentCandidateServiceMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IJobApplicationStageProgressRepository> _jobApplicationStageProgressRepositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly Mock<IApplicationSettingService> _applicationSettingServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly JobApplicationSelfService _service;

    public JobApplicationSelfServiceTests()
    {
        _currentCandidateServiceMock = new Mock<ICurrentCandidateService>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _jobApplicationStageProgressRepositoryMock = new Mock<IJobApplicationStageProgressRepository>();
        _jobPostingRepositoryMock = new Mock<IJobPostingRepository>();
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();
        _applicationSettingServiceMock = new Mock<IApplicationSettingService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _currentCandidateServiceMock.Setup(s => s.GetCurrentEmailAsync()).ReturnsAsync("jane@example.com");

        // GetMyApplicationsAsync's per-application stage-progress merge (US-040) - no in-flight
        // rows by default, so existing tests that don't set up stage-progress fixtures see an
        // empty tracker list instead of a null-source ArgumentNullException.
        _jobApplicationStageProgressRepositoryMock
            .Setup(r => r.GetByJobApplicationIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new List<JobApplicationStageProgress>());

        var dispatchTargetBuilder = new JobApplicationDispatchTargetBuilder(_applicationSettingServiceMock.Object);

        _service = new JobApplicationSelfService(
            _currentCandidateServiceMock.Object,
            _jobApplicationRepositoryMock.Object,
            _jobApplicationStageProgressRepositoryMock.Object,
            _jobPostingRepositoryMock.Object,
            _candidateProfileRepositoryMock.Object,
            _unitOfWorkMock.Object,
            Mock.Of<ILogger<JobApplicationSelfService>>(),
            Mock.Of<INotificationDispatchService>(),
            dispatchTargetBuilder);
    }

    // ── GetMyApplicationsAsync / WithdrawMyApplicationAsync (US-040) ──────

    [Fact]
    public async Task GetMyApplicationsAsync_ShouldReturnApplicationsForCurrentCandidateEmailWithCanWithdrawFlag()
    {
        // Arrange
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open };
        var applications = new List<JobApplication>
        {
            new()
            {
                JobApplicationId = 1,
                JobPostingId = 1,
                JobPosting = jobPosting,
                CandidateEmail = "jane@example.com",
                ApplicationStatus = ApplicationStatusEnum.Screening,
                Interviews = new List<Interview>()
            },
            new()
            {
                JobApplicationId = 2,
                JobPostingId = 1,
                JobPosting = jobPosting,
                CandidateEmail = "jane@example.com",
                ApplicationStatus = ApplicationStatusEnum.Hired,
                Interviews = new List<Interview>()
            }
        };
        _jobApplicationRepositoryMock.Setup(r => r.GetByCandidateAsync(It.IsAny<long?>(), "jane@example.com")).ReturnsAsync(applications);

        // Act
        var result = await _service.GetMyApplicationsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Single(a => a.JobApplicationId == 1).CanWithdraw.Should().BeTrue();
        result.Single(a => a.JobApplicationId == 2).CanWithdraw.Should().BeFalse();
    }

    [Fact]
    public async Task WithdrawMyApplicationAsync_WithOwnActiveApplication_ShouldSetWithdrawnAndRecordHistory()
    {
        // Arrange
        var entity = new JobApplication { JobApplicationId = 1, JobPostingId = 10, CandidateEmail = "jane@example.com", ApplicationStatus = ApplicationStatusEnum.Screening };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new JobPosting { JobPostingId = 10, Status = JobStatusEnum.Open });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.WithdrawMyApplicationAsync(1);

        // Assert
        entity.ApplicationStatus.Should().Be(ApplicationStatusEnum.Withdrawn);
        entity.StatusHistory.Should().ContainSingle();
        entity.StatusHistory.Single().ToStatus.Should().Be(ApplicationStatusEnum.Withdrawn);
        entity.StatusHistory.Single().ReasonId.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task WithdrawMyApplicationAsync_WithAnotherCandidatesApplication_ShouldThrowNotFoundException()
    {
        // Arrange: entity belongs to a different candidate than the caller (jane@example.com).
        var entity = new JobApplication { JobApplicationId = 1, CandidateEmail = "someoneelse@example.com", ApplicationStatus = ApplicationStatusEnum.Screening };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        var act = () => _service.WithdrawMyApplicationAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task WithdrawMyApplicationAsync_WithTerminalStatus_ShouldThrowInvalidStatusTransitionException()
    {
        // Arrange: Hired is terminal, cannot transition to Withdrawn.
        var entity = new JobApplication { JobApplicationId = 1, CandidateEmail = "jane@example.com", ApplicationStatus = ApplicationStatusEnum.Hired };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        var act = () => _service.WithdrawMyApplicationAsync(1);

        // Assert
        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task WithdrawMyApplicationAsync_WithClosedJobPosting_ShouldThrowInvalidStatusTransitionException()
    {
        // Arrange: application status itself is withdrawable, but the job posting has closed.
        var entity = new JobApplication { JobApplicationId = 1, JobPostingId = 10, CandidateEmail = "jane@example.com", ApplicationStatus = ApplicationStatusEnum.Screening };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new JobPosting { JobPostingId = 10, Status = JobStatusEnum.Closed });

        // Act
        var act = () => _service.WithdrawMyApplicationAsync(1);

        // Assert
        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task WithdrawMyApplicationAsync_AlreadyWithdrawn_ShouldBeIdempotentNoOp()
    {
        // Arrange
        var entity = new JobApplication { JobApplicationId = 1, CandidateEmail = "jane@example.com", ApplicationStatus = ApplicationStatusEnum.Withdrawn };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        await _service.WithdrawMyApplicationAsync(1);

        // Assert
        entity.StatusHistory.Should().BeEmpty();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }


    [Fact]
    public async Task CheckEligibilityAsync_JobPostingNotFound_ShouldThrowNotFoundException()
    {
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((JobPosting?)null);

        var act = async () => await _service.CheckEligibilityAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CheckEligibilityAsync_ProfileMeetsAllRequirements_ShouldReturnEligible()
    {
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", MinAge = 21, RequiredDistrict = "Dhaka" };
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(jobPosting);

        _currentCandidateServiceMock.Setup(s => s.GetOrCreateCurrentProfileIdAsync()).ReturnsAsync(5);

        var profile = new CandidateProfile
        {
            CandidateProfileId = 5,
            DateOfBirth = DateTime.UtcNow.AddYears(-25),
            PresentAddressDetail = "House 1, Dhanmondi, Dhaka"
        };
        _candidateProfileRepositoryMock
            .Setup(r => r.GetByIdWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<CandidateProfile, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<CandidateProfile, object>>[]>()))
            .ReturnsAsync(profile);

        var result = await _service.CheckEligibilityAsync(1);

        result.IsEligible.Should().BeTrue();
        result.UnmetRequirements.Should().BeEmpty();
    }

    [Fact]
    public async Task CheckEligibilityAsync_ProfileMissesRequirements_ShouldReturnUnmetReasons()
    {
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", MinAge = 30, RequiredDistrict = "Dhaka" };
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(jobPosting);

        _currentCandidateServiceMock.Setup(s => s.GetOrCreateCurrentProfileIdAsync()).ReturnsAsync(5);

        var profile = new CandidateProfile
        {
            CandidateProfileId = 5,
            DateOfBirth = DateTime.UtcNow.AddYears(-22),
            PresentAddressDetail = "House 1, Agrabad, Chattogram"
        };
        _candidateProfileRepositoryMock
            .Setup(r => r.GetByIdWithIncludeAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<CandidateProfile, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<CandidateProfile, object>>[]>()))
            .ReturnsAsync(profile);

        var result = await _service.CheckEligibilityAsync(1);

        result.IsEligible.Should().BeFalse();
        result.UnmetRequirements.Should().HaveCount(2);
    }

}
