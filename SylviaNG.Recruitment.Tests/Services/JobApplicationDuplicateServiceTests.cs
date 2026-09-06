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

public class JobApplicationDuplicateServiceTests
{
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<IApplicationStatusReasonRepository> _statusReasonRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly Mock<ICvPdfGeneratorService> _cvPdfGeneratorServiceMock;
    private readonly Mock<IApplicationSettingService> _applicationSettingServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly JobApplicationDuplicateService _service;

    public JobApplicationDuplicateServiceTests()
    {
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _jobPostingRepositoryMock = new Mock<IJobPostingRepository>();
        _statusReasonRepositoryMock = new Mock<IApplicationStatusReasonRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();
        _cvPdfGeneratorServiceMock = new Mock<ICvPdfGeneratorService>();
        _applicationSettingServiceMock = new Mock<IApplicationSettingService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _currentUserServiceMock.Setup(s => s.GetCurrentUserName()).Returns("abir");

        var dispatchTargetBuilder = new JobApplicationDispatchTargetBuilder(_applicationSettingServiceMock.Object);

        // Real (not mocked) JobApplicationStatusService - ResolveDuplicatesAsync calls its
        // ApplyStatusChangeAsync to actually perform the dismiss transition, and these tests
        // assert on the resulting entity mutation (ApplicationStatus/StatusHistory), so a bare
        // mock (which would no-op) can't stand in here the way it can for callers that only
        // trigger the call without inspecting its effect.
        var statusService = new JobApplicationStatusService(
            _jobApplicationRepositoryMock.Object,
            _jobPostingRepositoryMock.Object,
            _statusReasonRepositoryMock.Object,
            _currentUserServiceMock.Object,
            Mock.Of<INotificationDispatchService>(),
            _candidateProfileRepositoryMock.Object,
            _cvPdfGeneratorServiceMock.Object,
            _unitOfWorkMock.Object,
            Mock.Of<ILogger<JobApplicationStatusService>>(),
            dispatchTargetBuilder);

        _service = new JobApplicationDuplicateService(
            _jobApplicationRepositoryMock.Object,
            _unitOfWorkMock.Object,
            statusService);
    }

    // ── GetDuplicatesAsync / ResolveDuplicatesAsync (US-038) ────────────────

    [Fact]
    public async Task GetDuplicatesAsync_WhenSameEmailAcrossSources_ShouldGroupThemAndReportEmailMatch()
    {
        var applications = new List<JobApplication>
        {
            new() { JobApplicationId = 1, JobPostingId = 1, CandidateEmail = "jane@example.com", CandidatePhone = "0111111111", Source = ApplicationSourceEnum.External },
            new() { JobApplicationId = 2, JobPostingId = 1, CandidateEmail = "Jane@Example.com", CandidatePhone = "0222222222", Source = ApplicationSourceEnum.Admin }
        };
        _jobApplicationRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(1)).ReturnsAsync(applications);

        var result = await _service.GetDuplicatesAsync(1);

        result.Should().ContainSingle();
        result[0].Applications.Select(a => a.JobApplicationId).Should().BeEquivalentTo(new long[] { 1, 2 });
        result[0].MatchedOn.Should().Contain("Email");
    }

    [Fact]
    public async Task GetDuplicatesAsync_WhenSamePhoneDifferentEmail_ShouldGroupThemAndReportPhoneMatch()
    {
        var applications = new List<JobApplication>
        {
            new() { JobApplicationId = 1, JobPostingId = 1, CandidateEmail = "a@example.com", CandidatePhone = "+880 171-234567" },
            new() { JobApplicationId = 2, JobPostingId = 1, CandidateEmail = "b@example.com", CandidatePhone = "01712-34567" }
        };
        _jobApplicationRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(1)).ReturnsAsync(applications);

        var result = await _service.GetDuplicatesAsync(1);

        result.Should().ContainSingle();
        result[0].MatchedOn.Should().Contain("Phone");
        result[0].MatchedOn.Should().NotContain("Email");
    }

    [Fact]
    public async Task GetDuplicatesAsync_WhenSameNationalId_ShouldGroupThemAndReportNationalIdMatch()
    {
        var applications = new List<JobApplication>
        {
            new() { JobApplicationId = 1, JobPostingId = 1, CandidateEmail = "a@example.com", CandidatePhone = "0111111111", CandidateNationalId = "NID-123" },
            new() { JobApplicationId = 2, JobPostingId = 1, CandidateEmail = "b@example.com", CandidatePhone = "0222222222", CandidateNationalId = "NID-123" }
        };
        _jobApplicationRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(1)).ReturnsAsync(applications);

        var result = await _service.GetDuplicatesAsync(1);

        result.Should().ContainSingle();
        result[0].MatchedOn.Should().Contain("NationalId");
    }

    [Fact]
    public async Task GetDuplicatesAsync_WhenNothingOverlaps_ShouldReturnNoGroups()
    {
        var applications = new List<JobApplication>
        {
            new() { JobApplicationId = 1, JobPostingId = 1, CandidateEmail = "a@example.com", CandidatePhone = "0111111111" },
            new() { JobApplicationId = 2, JobPostingId = 1, CandidateEmail = "b@example.com", CandidatePhone = "0222222222" }
        };
        _jobApplicationRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(1)).ReturnsAsync(applications);

        var result = await _service.GetDuplicatesAsync(1);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDuplicatesAsync_WhenOnlyOneMemberRemainsNonDismissed_ShouldNotSurfaceAsOpenGroup()
    {
        // Arrange: a pair was already resolved (one dismissed) - shouldn't keep reappearing.
        var applications = new List<JobApplication>
        {
            new() { JobApplicationId = 1, JobPostingId = 1, CandidateEmail = "jane@example.com", ApplicationStatus = ApplicationStatusEnum.Applied },
            new() { JobApplicationId = 2, JobPostingId = 1, CandidateEmail = "jane@example.com", ApplicationStatus = ApplicationStatusEnum.DuplicateDismissed }
        };
        _jobApplicationRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(1)).ReturnsAsync(applications);

        var result = await _service.GetDuplicatesAsync(1);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ResolveDuplicatesAsync_ShouldDismissNonPrimaryAndLeavePrimaryUntouchedWithAuditNote()
    {
        var primary = new JobApplication { JobApplicationId = 1, JobPostingId = 1, CandidateEmail = "jane@example.com", ApplicationStatus = ApplicationStatusEnum.Applied };
        var duplicate = new JobApplication { JobApplicationId = 2, JobPostingId = 1, CandidateEmail = "jane@example.com", ApplicationStatus = ApplicationStatusEnum.Applied };

        _jobApplicationRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(1)).ReturnsAsync(new List<JobApplication> { primary, duplicate });
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(duplicate);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = new JobApplicationDuplicateResolveRequest
        {
            JobPostingId = 1,
            PrimaryJobApplicationId = 1,
            DuplicateJobApplicationIds = new List<long> { 2 }
        };

        await _service.ResolveDuplicatesAsync(request);

        primary.ApplicationStatus.Should().Be(ApplicationStatusEnum.Applied);
        duplicate.ApplicationStatus.Should().Be(ApplicationStatusEnum.DuplicateDismissed);
        duplicate.StatusHistory.Should().ContainSingle();
        duplicate.StatusHistory.Single().Note.Should().Contain("#1");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ResolveDuplicatesAsync_WhenSuppliedIdIsNotInDetectedGroup_ShouldThrowValidationException()
    {
        var primary = new JobApplication { JobApplicationId = 1, JobPostingId = 1, CandidateEmail = "jane@example.com" };
        var unrelated = new JobApplication { JobApplicationId = 3, JobPostingId = 1, CandidateEmail = "someone-else@example.com" };

        _jobApplicationRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(1)).ReturnsAsync(new List<JobApplication> { primary, unrelated });

        var request = new JobApplicationDuplicateResolveRequest
        {
            JobPostingId = 1,
            PrimaryJobApplicationId = 1,
            DuplicateJobApplicationIds = new List<long> { 3 }
        };

        var act = () => _service.ResolveDuplicatesAsync(request);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ResolveDuplicatesAsync_WhenNoDuplicateIdsSupplied_ShouldThrowValidationException()
    {
        var request = new JobApplicationDuplicateResolveRequest
        {
            JobPostingId = 1,
            PrimaryJobApplicationId = 1,
            DuplicateJobApplicationIds = new List<long>()
        };

        var act = () => _service.ResolveDuplicatesAsync(request);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

}
