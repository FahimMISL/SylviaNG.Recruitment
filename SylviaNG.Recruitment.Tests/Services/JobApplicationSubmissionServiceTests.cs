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

public class JobApplicationSubmissionServiceTests
{
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly Mock<ICandidateDocumentRepository> _candidateDocumentRepositoryMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IApplicationCvStorageService> _cvStorageServiceMock;
    private readonly Mock<ICurrentCandidateService> _currentCandidateServiceMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly Mock<IWaiverRuleService> _waiverRuleServiceMock;
    private readonly Mock<IApplicationSettingService> _applicationSettingServiceMock;
    private readonly Mock<IResumeParsingService> _resumeParsingServiceMock;
    private readonly Mock<INotificationDispatchQueue> _notificationDispatchQueueMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly JobApplicationSubmissionService _service;

    public JobApplicationSubmissionServiceTests()
    {
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _jobPostingRepositoryMock = new Mock<IJobPostingRepository>();
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();
        _candidateDocumentRepositoryMock = new Mock<ICandidateDocumentRepository>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _cvStorageServiceMock = new Mock<IApplicationCvStorageService>();
        _currentCandidateServiceMock = new Mock<ICurrentCandidateService>();
        _paymentServiceMock = new Mock<IPaymentService>();
        _waiverRuleServiceMock = new Mock<IWaiverRuleService>();
        _applicationSettingServiceMock = new Mock<IApplicationSettingService>();
        _resumeParsingServiceMock = new Mock<IResumeParsingService>();
        _notificationDispatchQueueMock = new Mock<INotificationDispatchQueue>();
        _notificationDispatchQueueMock.Setup(q => q.TryEnqueue(It.IsAny<NotificationDispatchRequest>())).Returns(true);
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _currentCandidateServiceMock.Setup(s => s.GetCurrentEmailAsync()).ReturnsAsync("jane@example.com");

        // Gate disabled by default (mirrors the seeded 0 = off), so existing SubmitAsync cases
        // that never set up profile-completeness fixtures are unaffected (US-007 AC4).
        _applicationSettingServiceMock.Setup(s => s.GetMinimumProfileCompletenessPercentageAsync()).ReturnsAsync(0);

        // No waiver rule matches by default, so existing SubmitAsync cases that never set up
        // waiver fixtures keep their original requiresPayment behavior (EP-17/US-127).
        _waiverRuleServiceMock
            .Setup(s => s.TryMatchAsync(It.IsAny<bool>(), It.IsAny<long?>(), It.IsAny<long?>()))
            .ReturnsAsync((WaiverRule?)null);

        // No existing resume document by default, so existing SubmitAsync cases that always pass
        // a Resume file are unaffected by the reuse-existing-resume fallback path.
        _candidateDocumentRepositoryMock
            .Setup(r => r.GetAllByCandidateProfileIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new List<CandidateDocument>());

        // Real (not mocked) small helpers extracted out of the former monolithic
        // JobApplicationService - they're pure/deterministic wrappers over the mocks above, so
        // using the real instances here keeps this test's assertions identical to what they were
        // before the split, rather than having to guess what a mock of them should return.
        var candidateApplicationLinkResolver = new CandidateApplicationLinkResolver(_currentCandidateServiceMock.Object, _candidateProfileRepositoryMock.Object);
        var dispatchTargetBuilder = new JobApplicationDispatchTargetBuilder(_applicationSettingServiceMock.Object);

        _service = new JobApplicationSubmissionService(
            _jobApplicationRepositoryMock.Object,
            _jobPostingRepositoryMock.Object,
            _candidateProfileRepositoryMock.Object,
            _candidateDocumentRepositoryMock.Object,
            _fileStorageServiceMock.Object,
            _cvStorageServiceMock.Object,
            _waiverRuleServiceMock.Object,
            _applicationSettingServiceMock.Object,
            _resumeParsingServiceMock.Object,
            _paymentServiceMock.Object,
            _notificationDispatchQueueMock.Object,
            _unitOfWorkMock.Object,
            Mock.Of<ILogger<JobApplicationSubmissionService>>(),
            candidateApplicationLinkResolver,
            dispatchTargetBuilder);
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
        // The response maps the stored path through the file-download proxy (MinIO migration), while
        // the persisted entity keeps the raw relative path (asserted via savedEntity elsewhere).
        result.ResumeUrl.Should().Be("recruitment/files/download?key=uploads%2Fapplications%2F1%2Fabc123.pdf");
        savedEntity.Should().NotBeNull();
        savedEntity!.Source.Should().Be(source);
        savedEntity.ApplicationStatus.Should().Be(ApplicationStatusEnum.Applied);
        savedEntity.AppliedDate.Should().NotBeNull();
        _jobApplicationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WithResume_ShouldPersistExtractedResumeText()
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
        _resumeParsingServiceMock.Setup(s => s.ExtractRawTextAsync(resume)).ReturnsAsync("Kubernetes expert with 5 years experience");

        JobApplication? savedEntity = null;
        _jobApplicationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<JobApplication>()))
            .Callback<JobApplication>(a => savedEntity = a);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.SubmitAsync(CreateRequest(resume: resume), ApplicationSourceEnum.External);

        // Assert
        savedEntity!.ResumeExtractedText.Should().Be("Kubernetes expert with 5 years experience");
    }

    [Fact]
    public async Task SubmitAsync_WhenResumeExtractionThrows_ShouldStillSaveApplicationWithNullExtractedText()
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
        _resumeParsingServiceMock.Setup(s => s.ExtractRawTextAsync(resume)).ThrowsAsync(new InvalidOperationException("corrupt PDF"));

        JobApplication? savedEntity = null;
        _jobApplicationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<JobApplication>()))
            .Callback<JobApplication>(a => savedEntity = a);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var act = () => _service.SubmitAsync(CreateRequest(resume: resume), ApplicationSourceEnum.External);

        // Assert - extraction failure must not fail submission
        await act.Should().NotThrowAsync();
        savedEntity!.ResumeExtractedText.Should().BeNull();
        savedEntity.ResumeUrl.Should().Be("uploads/applications/1/abc123.pdf");
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


    [Theory]
    [InlineData(ApplicationSourceEnum.External)]
    [InlineData(ApplicationSourceEnum.Internal)]
    public async Task SubmitAsync_WithApplicationFeeConfigured_ShouldSetAwaitingPaymentAndInitiatePayment(ApplicationSourceEnum source)
    {
        // Arrange
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open, ApplicationFeeAmount = 500m, ApplicationFeeCurrency = "BDT" };
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
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

        _paymentServiceMock.Setup(p => p.InitiateAsync(10, "jane@example.com"))
            .ReturnsAsync(new PaymentInitiateResponse { Success = true, GatewayRedirectUrl = "https://sandbox.sslcommerz.com/pay/abc" });

        var request = CreateRequest(resume: null);

        // Act
        var result = await _service.SubmitAsync(request, source);

        // Assert
        savedEntity!.ApplicationStatus.Should().Be(ApplicationStatusEnum.AwaitingPayment);
        result.PaymentRequired.Should().BeTrue();
        result.PaymentRedirectUrl.Should().Be("https://sandbox.sslcommerz.com/pay/abc");
        _paymentServiceMock.Verify(p => p.InitiateAsync(10, "jane@example.com"), Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WithMatchingWaiverRule_ShouldBypassPaymentAndStampWaiverFields()
    {
        // Arrange: EP-17/US-127 - a matching waiver rule should skip payment the same way HR
        // apply-on-behalf does, even though the vacancy has a fee configured.
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open, ApplicationFeeAmount = 500m, ApplicationFeeCurrency = "BDT" };
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
            .ReturnsAsync(jobPosting);

        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync((JobApplication?)null);

        var matchedRule = new WaiverRule { WaiverRuleId = 7, Name = "Internal Staff", CandidateTypeFilter = WaiverCandidateTypeEnum.Internal };
        _waiverRuleServiceMock
            .Setup(s => s.TryMatchAsync(It.IsAny<bool>(), It.IsAny<long?>(), It.IsAny<long?>()))
            .ReturnsAsync(matchedRule);

        JobApplication? savedEntity = null;
        _jobApplicationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<JobApplication>()))
            .Callback<JobApplication>(a =>
            {
                a.JobApplicationId = 10;
                savedEntity = a;
            });

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // A matched rule only waives the fee when proof is attached (JobApplicationService.
        // SubmitAsync nulls waiverRule back out otherwise) - without this the test's own claimed
        // scenario (fee waived) can't actually happen, and requiresPayment falls through to the
        // unmocked payment gateway call instead.
        var waiverProof = CreateFormFile("waiver-proof.pdf");
        _cvStorageServiceMock
            .Setup(s => s.SaveAsync(It.IsAny<Stream>(), "waiver-proof.pdf", "1"))
            .ReturnsAsync(("proof123.pdf", "uploads/applications/1/proof123.pdf"));

        var request = CreateRequest(resume: null);
        request.WaiverProofDocument = waiverProof;

        // Act
        var result = await _service.SubmitAsync(request, ApplicationSourceEnum.External);

        // Assert
        savedEntity!.ApplicationStatus.Should().Be(ApplicationStatusEnum.Applied);
        savedEntity.WaiverRuleId.Should().Be(7);
        savedEntity.WaivedAt.Should().NotBeNull();
        result.PaymentRequired.Should().BeFalse();
        result.PaymentRedirectUrl.Should().BeNull();
        _paymentServiceMock.Verify(p => p.InitiateAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        savedEntity.StatusHistory.Should().ContainSingle(h => h.ChangedByUserName == "system:fee-waiver" && h.ToStatus == ApplicationStatusEnum.Applied);
    }

    [Fact]
    public async Task SubmitAsync_WithAdminSourceOnFeeConfiguredPosting_ShouldBypassPayment()
    {
        // Arrange: HR applying on behalf shouldn't be forced through an SSLCommerz checkout.
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open, ApplicationFeeAmount = 500m, ApplicationFeeCurrency = "BDT" };
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
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
        var result = await _service.SubmitAsync(request, ApplicationSourceEnum.Admin);

        // Assert
        savedEntity!.ApplicationStatus.Should().Be(ApplicationStatusEnum.Applied);
        result.PaymentRequired.Should().BeFalse();
        result.PaymentRedirectUrl.Should().BeNull();
        _paymentServiceMock.Verify(p => p.InitiateAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WithNoFeeConfigured_ShouldApplyImmediatelyWithoutInvokingPaymentService()
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
        var result = await _service.SubmitAsync(request, ApplicationSourceEnum.External);

        // Assert
        savedEntity!.ApplicationStatus.Should().Be(ApplicationStatusEnum.Applied);
        result.PaymentRequired.Should().BeFalse();
        _paymentServiceMock.Verify(p => p.InitiateAsync(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WhenPaymentGatewayUnavailable_ShouldStillSaveApplicationWithNullRedirectUrl()
    {
        // Arrange: a gateway outage must not roll back the application that's already saved -
        // the candidate should be able to retry payment later (PaymentController.Initiate).
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open, ApplicationFeeAmount = 500m, ApplicationFeeCurrency = "BDT" };
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
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

        _paymentServiceMock.Setup(p => p.InitiateAsync(10, "jane@example.com"))
            .ThrowsAsync(new SslCommerzUnavailableException("SSLCommerz gateway is unreachable."));

        var request = CreateRequest(resume: null);

        // Act
        var result = await _service.SubmitAsync(request, ApplicationSourceEnum.External);

        // Assert
        savedEntity.Should().NotBeNull();
        savedEntity!.ApplicationStatus.Should().Be(ApplicationStatusEnum.AwaitingPayment);
        result.PaymentRequired.Should().BeTrue();
        result.PaymentRedirectUrl.Should().BeNull();
        _jobApplicationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Once);
    }

    // ── US-007 AC4: minimum profile completeness gate on submit ───────────

    private static void SetUpOpenPosting(Mock<IJobPostingRepository> jobPostingRepositoryMock)
    {
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open };
        jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
            .ReturnsAsync(jobPosting);
    }

    [Theory]
    [InlineData(ApplicationSourceEnum.External)]
    [InlineData(ApplicationSourceEnum.Internal)]
    public async Task SubmitAsync_WhenProfileBelowConfiguredThreshold_ShouldThrowValidationException(ApplicationSourceEnum source)
    {
        SetUpOpenPosting(_jobPostingRepositoryMock);
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync((JobApplication?)null);
        _applicationSettingServiceMock.Setup(s => s.GetMinimumProfileCompletenessPercentageAsync()).ReturnsAsync(50);
        _candidateProfileRepositoryMock
            .Setup(r => r.GetByEmailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<CandidateProfile> { new() { Email = "jane@example.com" } }); // 0/7 sections = 0%

        var act = () => _service.SubmitAsync(CreateRequest(), source);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        _jobApplicationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WhenProfileMeetsConfiguredThreshold_ShouldSucceed()
    {
        SetUpOpenPosting(_jobPostingRepositoryMock);
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync((JobApplication?)null);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _applicationSettingServiceMock.Setup(s => s.GetMinimumProfileCompletenessPercentageAsync()).ReturnsAsync(50);
        _candidateProfileRepositoryMock
            .Setup(r => r.GetByEmailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<CandidateProfile>
            {
                new()
                {
                    FullName = "Jane Doe",
                    Email = "jane@example.com",
                    DateOfBirth = new DateTime(1995, 1, 1),
                    Phone = "+880123456789",
                    PresentAddressDetail = "Dhaka",
                    Educations = new List<CandidateEducation> { new() },
                    WorkExperiences = new List<CandidateWorkExperience> { new() }
                } // FullName/Email/Educations/WorkExperiences = 4/7 sections ≈ 57%
            });

        var result = await _service.SubmitAsync(CreateRequest(), ApplicationSourceEnum.External);

        result.Should().NotBeNull();
        _jobApplicationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WhenCandidateHasNoProfile_ShouldBypassGate()
    {
        // Guest apply via career portal never creates a CandidateProfile - nothing to measure
        // against, so a configured threshold must not block a plain anonymous applicant.
        SetUpOpenPosting(_jobPostingRepositoryMock);
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync((JobApplication?)null);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _applicationSettingServiceMock.Setup(s => s.GetMinimumProfileCompletenessPercentageAsync()).ReturnsAsync(50);
        _candidateProfileRepositoryMock
            .Setup(r => r.GetByEmailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<CandidateProfile>());

        var result = await _service.SubmitAsync(CreateRequest(), ApplicationSourceEnum.External);

        result.Should().NotBeNull();
        _jobApplicationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WithAdminSource_ShouldBypassCompletenessGateEvenBelowThreshold()
    {
        SetUpOpenPosting(_jobPostingRepositoryMock);
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync((JobApplication?)null);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _applicationSettingServiceMock.Setup(s => s.GetMinimumProfileCompletenessPercentageAsync()).ReturnsAsync(90);
        _candidateProfileRepositoryMock
            .Setup(r => r.GetByEmailsAsync(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new List<CandidateProfile> { new() { Email = "jane@example.com" } }); // 0%

        var result = await _service.SubmitAsync(CreateRequest(), ApplicationSourceEnum.Admin);

        result.Should().NotBeNull();
        _candidateProfileRepositoryMock.Verify(r => r.GetByEmailsAsync(It.IsAny<IEnumerable<string>>()), Times.Never);
    }

}
