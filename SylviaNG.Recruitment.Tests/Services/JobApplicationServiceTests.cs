using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Domain.Events;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class JobApplicationServiceTests
{
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<IApplicationCvStorageService> _cvStorageServiceMock;
    private readonly Mock<IApplicationStatusReasonRepository> _statusReasonRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<ICurrentCandidateService> _currentCandidateServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly JobApplicationService _service;

    public JobApplicationServiceTests()
    {
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _jobPostingRepositoryMock = new Mock<IJobPostingRepository>();
        _cvStorageServiceMock = new Mock<IApplicationCvStorageService>();
        _statusReasonRepositoryMock = new Mock<IApplicationStatusReasonRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentCandidateServiceMock = new Mock<ICurrentCandidateService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _currentUserServiceMock.Setup(s => s.GetCurrentUserName()).Returns("abir");
        _currentCandidateServiceMock.Setup(s => s.GetCurrentEmailAsync()).ReturnsAsync("jane@example.com");

        _service = new JobApplicationService(
            _jobApplicationRepositoryMock.Object,
            _jobPostingRepositoryMock.Object,
            _cvStorageServiceMock.Object,
            _statusReasonRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _currentCandidateServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private static IFormFile CreateFormFile(string fileName = "resume.pdf", string content = "dummy content")
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "resume", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };
    }

    private static JobApplicationSubmitRequest CreateRequest(long jobPostingId = 1, string email = "jane@example.com", IFormFile? resume = null)
    {
        return new JobApplicationSubmitRequest
        {
            JobPostingId = jobPostingId,
            CandidateName = "Jane Doe",
            CandidateEmail = email,
            CandidatePhone = "+880123456789",
            CoverLetter = "I am interested in this role.",
            Resume = resume
        };
    }

    [Theory]
    [InlineData(ApplicationSourceEnum.External)]
    [InlineData(ApplicationSourceEnum.Internal)]
    [InlineData(ApplicationSourceEnum.Admin)]
    public async Task SubmitAsync_WithValidRequestAndOpenMatchingPosting_ShouldSaveApplicationAndReturnResponse(ApplicationSourceEnum source)
    {
        // Arrange
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open };
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
            .ReturnsAsync(jobPosting);

        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync((JobApplication?)null);

        var resume = CreateFormFile();
        _cvStorageServiceMock
            .Setup(s => s.SaveAsync(It.IsAny<Stream>(), "resume.pdf", "1"))
            .ReturnsAsync(("abc123.pdf", "uploads/applications/1/abc123.pdf"));

        JobApplication? savedEntity = null;
        _jobApplicationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<JobApplication>()))
            .Callback<JobApplication>(a =>
            {
                a.JobApplicationId = 10;
                savedEntity = a;
            });

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = CreateRequest(resume: resume);

        // Act
        var result = await _service.SubmitAsync(request, source);

        // Assert
        result.Should().NotBeNull();
        result.JobApplicationId.Should().Be(10);
        result.Source.Should().Be(source);
        result.ResumeUrl.Should().Be("uploads/applications/1/abc123.pdf");
        savedEntity.Should().NotBeNull();
        savedEntity!.Source.Should().Be(source);
        savedEntity.ApplicationStatus.Should().Be(ApplicationStatusEnum.Applied);
        savedEntity.AppliedDate.Should().NotBeNull();
        _jobApplicationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WithoutResume_ShouldNotCallCvStorageAndShouldSaveWithNullResumeUrl()
    {
        // Arrange
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open };
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
            .ReturnsAsync(jobPosting);

        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync((JobApplication?)null);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = CreateRequest(resume: null);

        // Act
        var result = await _service.SubmitAsync(request, ApplicationSourceEnum.External);

        // Assert
        result.ResumeUrl.Should().BeNull();
        _cvStorageServiceMock.Verify(s => s.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WithDuplicateEmailForSameJobPosting_ShouldThrowDuplicateException()
    {
        // Arrange
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open };
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
            .ReturnsAsync(jobPosting);

        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync(new JobApplication { JobApplicationId = 5, JobPostingId = 1, CandidateEmail = "jane@example.com" });

        var request = CreateRequest();

        // Act
        var act = () => _service.SubmitAsync(request, ApplicationSourceEnum.External);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
        _jobApplicationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Never);
    }

    [Theory]
    [InlineData(ApplicationSourceEnum.External)]
    [InlineData(ApplicationSourceEnum.Internal)]
    public async Task SubmitAsync_WithNoMatchingOpenPostingForAudience_ShouldThrowNotFoundException(ApplicationSourceEnum source)
    {
        // Arrange: repository returns null when the posting is not Open or the CircularType
        // doesn't match the requested audience (e.g. an InternalOnly posting reached via the
        // public career portal, or vice-versa).
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
            .ReturnsAsync((JobPosting?)null);

        var request = CreateRequest();

        // Act
        var act = () => _service.SubmitAsync(request, source);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _jobApplicationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    // ── SubmitAsync with Admin source (US-034) ────────────────────────────

    [Fact]
    public async Task SubmitAsync_WithAdminSource_ShouldAllowAnyCircularTypeAndRaiseNotificationEvent()
    {
        // Arrange: an InternalOnly posting, which External/Internal-sourced applies could not reach,
        // but HR applying on behalf should be able to (US-034 AC1: "any open vacancy").
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open, CircularType = CircularTypeEnum.InternalOnly };

        IReadOnlyCollection<CircularTypeEnum>? capturedCircularTypes = null;
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
            .Callback<long, IReadOnlyCollection<CircularTypeEnum>>((_, types) => capturedCircularTypes = types)
            .ReturnsAsync(jobPosting);

        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync((JobApplication?)null);

        JobApplication? savedEntity = null;
        _jobApplicationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<JobApplication>()))
            .Callback<JobApplication>(a =>
            {
                a.JobApplicationId = 10;
                savedEntity = a;
            });

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = CreateRequest(resume: null);

        // Act
        await _service.SubmitAsync(request, ApplicationSourceEnum.Admin);

        // Assert
        capturedCircularTypes.Should().Contain(new[] { CircularTypeEnum.ExternalOnly, CircularTypeEnum.InternalOnly, CircularTypeEnum.Both });
        savedEntity.Should().NotBeNull();
        savedEntity!.DomainEvents.Should().ContainSingle(e => e is JobApplicationSubmittedOnBehalfEvent);
        var raisedEvent = (JobApplicationSubmittedOnBehalfEvent)savedEntity.DomainEvents.Single();
        raisedEvent.JobApplicationId.Should().Be(10);
        raisedEvent.CandidateEmail.Should().Be("jane@example.com");
    }

    [Theory]
    [InlineData(ApplicationSourceEnum.External)]
    [InlineData(ApplicationSourceEnum.Internal)]
    public async Task SubmitAsync_WithNonAdminSource_ShouldNotRaiseNotificationEvent(ApplicationSourceEnum source)
    {
        // Arrange
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open };
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
            .ReturnsAsync(jobPosting);

        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync((JobApplication?)null);

        JobApplication? savedEntity = null;
        _jobApplicationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<JobApplication>()))
            .Callback<JobApplication>(a => savedEntity = a);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = CreateRequest(resume: null);

        // Act
        await _service.SubmitAsync(request, source);

        // Assert
        savedEntity!.DomainEvents.Should().BeEmpty();
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

    // ── GetMyApplicationsAsync / WithdrawMyApplicationAsync (US-040) ──────

    [Fact]
    public async Task GetMyApplicationsAsync_ShouldReturnApplicationsForCurrentCandidateEmailWithCanWithdrawFlag()
    {
        // Arrange
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer" };
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
        _jobApplicationRepositoryMock.Setup(r => r.GetByCandidateEmailAsync("jane@example.com")).ReturnsAsync(applications);

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
        var entity = new JobApplication { JobApplicationId = 1, CandidateEmail = "jane@example.com", ApplicationStatus = ApplicationStatusEnum.Screening };
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
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
}
