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

public class JobApplicationStatusServiceTests
{
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<IApplicationStatusReasonRepository> _statusReasonRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly Mock<ICvPdfGeneratorService> _cvPdfGeneratorServiceMock;
    private readonly Mock<IApplicationSettingService> _applicationSettingServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly JobApplicationStatusService _service;

    public JobApplicationStatusServiceTests()
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

        _service = new JobApplicationStatusService(
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
    }

    // ── UpdateStatusAsync (US-036) ────────────────────────────────────────

    [Fact]
    public async Task UpdateStatusAsync_WithLegalTransition_ShouldUpdateStatusAndRecordHistory()
    {
        // Arrange
        var entity = new JobApplication { JobApplicationId = 1, ApplicationStatus = ApplicationStatusEnum.Applied };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = new JobApplicationStatusUpdateRequest { ToStatus = ApplicationStatusEnum.Screening };

        // Act
        await _service.UpdateStatusAsync(1, request);

        // Assert
        entity.ApplicationStatus.Should().Be(ApplicationStatusEnum.Screening);
        entity.StatusHistory.Should().ContainSingle();
        var history = entity.StatusHistory.Single();
        history.FromStatus.Should().Be(ApplicationStatusEnum.Applied);
        history.ToStatus.Should().Be(ApplicationStatusEnum.Screening);
        history.ChangedByUserName.Should().Be("abir");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_AppliedDirectlyToShortlisted_ShouldSucceed()
    {
        // Arrange: US-044's automated filter apply must be able to fast-track Applied straight to
        // Shortlisted without a manual Screening bump first.
        var entity = new JobApplication { JobApplicationId = 1, ApplicationStatus = ApplicationStatusEnum.Applied };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = new JobApplicationStatusUpdateRequest { ToStatus = ApplicationStatusEnum.Shortlisted };

        await _service.UpdateStatusAsync(1, request);

        entity.ApplicationStatus.Should().Be(ApplicationStatusEnum.Shortlisted);
    }

    [Fact]
    public async Task UpdateStatusAsync_WithIllegalTransition_ShouldThrowInvalidStatusTransitionException()
    {
        // Arrange: Applied cannot jump straight to Hired.
        var entity = new JobApplication { JobApplicationId = 1, ApplicationStatus = ApplicationStatusEnum.Applied };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var request = new JobApplicationStatusUpdateRequest { ToStatus = ApplicationStatusEnum.Hired };

        // Act
        var act = () => _service.UpdateStatusAsync(1, request);

        // Assert
        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_ToRejectedWithoutReason_ShouldThrowValidationException()
    {
        // Arrange
        var entity = new JobApplication { JobApplicationId = 1, ApplicationStatus = ApplicationStatusEnum.Screening };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var request = new JobApplicationStatusUpdateRequest { ToStatus = ApplicationStatusEnum.Rejected };

        // Act
        var act = () => _service.UpdateStatusAsync(1, request);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_ToRejectedWithReason_ShouldSucceed()
    {
        // Arrange
        var entity = new JobApplication { JobApplicationId = 1, ApplicationStatus = ApplicationStatusEnum.Screening };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = new JobApplicationStatusUpdateRequest { ToStatus = ApplicationStatusEnum.Rejected, ReasonId = 2, Note = "Not a fit" };

        // Act
        await _service.UpdateStatusAsync(1, request);

        // Assert
        entity.ApplicationStatus.Should().Be(ApplicationStatusEnum.Rejected);
        entity.StatusHistory.Single().ReasonId.Should().Be(2);
        entity.StatusHistory.Single().Note.Should().Be("Not a fit");
    }

    [Fact]
    public async Task UpdateStatusAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        // Arrange
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((JobApplication?)null);

        var request = new JobApplicationStatusUpdateRequest { ToStatus = ApplicationStatusEnum.Screening };

        // Act
        var act = () => _service.UpdateStatusAsync(99, request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ── BulkUpdateStatusAsync (US-035 AC5) ─────────────────────────────────

    [Fact]
    public async Task BulkUpdateStatusAsync_WithMixOfValidAndInvalidIds_ShouldPartiallySucceed()
    {
        // Arrange: id 1 is a legal transition, id 2 is illegal, id 3 doesn't exist.
        var entity1 = new JobApplication { JobApplicationId = 1, ApplicationStatus = ApplicationStatusEnum.Applied };
        var entity2 = new JobApplication { JobApplicationId = 2, ApplicationStatus = ApplicationStatusEnum.Hired };

        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity1);
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(entity2);
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync((JobApplication?)null);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = new JobApplicationBulkStatusUpdateRequest
        {
            JobApplicationIds = new List<long> { 1, 2, 3 },
            ToStatus = ApplicationStatusEnum.Screening
        };

        // Act
        var result = await _service.BulkUpdateStatusAsync(request);

        // Assert
        result.SucceededIds.Should().ContainSingle().Which.Should().Be(1);
        result.Failed.Should().HaveCount(2);
        result.Failed.Select(f => f.JobApplicationId).Should().BeEquivalentTo(new long[] { 2, 3 });
        entity1.ApplicationStatus.Should().Be(ApplicationStatusEnum.Screening);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task BulkDownloadCvsAsync_ApplicationWithProfile_ShouldReturnOneEntryZip()
    {
        var applications = new List<JobApplication>
        {
            new() { JobApplicationId = 1, CandidateProfileId = 100, CandidateName = "Jane Doe" }
        };
        _jobApplicationRepositoryMock.Setup(r => r.Query(It.IsAny<bool>())).Returns(applications.AsQueryable());

        var profile = new CandidateProfile { CandidateProfileId = 100, FullName = "Jane Doe" };
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdsWithDetailsAsync(It.Is<IEnumerable<long>>(ids => ids.Contains(100))))
            .ReturnsAsync(new List<CandidateProfile> { profile });
        _cvPdfGeneratorServiceMock.Setup(g => g.Generate(profile)).ReturnsAsync(System.Text.Encoding.UTF8.GetBytes("pdf-bytes"));

        var result = await _service.BulkDownloadCvsAsync(new JobApplicationCvBulkDownloadRequest { JobApplicationIds = new List<long> { 1 } });

        result.ContentType.Should().Be("application/zip");
        using var archive = new System.IO.Compression.ZipArchive(new MemoryStream(result.Content), System.IO.Compression.ZipArchiveMode.Read);
        archive.Entries.Should().ContainSingle(e => e.Name == "Jane_Doe_1.pdf");
    }

    [Fact]
    public async Task BulkDownloadCvsAsync_GuestApplicantWithNoProfile_ShouldSkipSilently()
    {
        var applications = new List<JobApplication>
        {
            new() { JobApplicationId = 2, CandidateProfileId = null, CandidateName = "Guest Applicant" }
        };
        _jobApplicationRepositoryMock.Setup(r => r.Query(It.IsAny<bool>())).Returns(applications.AsQueryable());

        var result = await _service.BulkDownloadCvsAsync(new JobApplicationCvBulkDownloadRequest { JobApplicationIds = new List<long> { 2 } });

        using var archive = new System.IO.Compression.ZipArchive(new MemoryStream(result.Content), System.IO.Compression.ZipArchiveMode.Read);
        archive.Entries.Should().BeEmpty();
        _cvPdfGeneratorServiceMock.Verify(g => g.Generate(It.IsAny<CandidateProfile>()), Times.Never);
    }

    [Fact]
    public async Task BulkDownloadCvsAsync_NoIds_ShouldThrowValidationException()
    {
        var act = () => _service.BulkDownloadCvsAsync(new JobApplicationCvBulkDownloadRequest { JobApplicationIds = new List<long>() });

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task BulkDownloadCvsAsync_ExceedsSyncMaxCount_ShouldThrowValidationException()
    {
        var ids = Enumerable.Range(1, JobApplicationStatusService.BulkDownloadCvsSyncMaxCount + 1).Select(i => (long)i).ToList();

        var act = () => _service.BulkDownloadCvsAsync(new JobApplicationCvBulkDownloadRequest { JobApplicationIds = ids });

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        _jobApplicationRepositoryMock.Verify(r => r.Query(It.IsAny<bool>()), Times.Never);
    }
}
