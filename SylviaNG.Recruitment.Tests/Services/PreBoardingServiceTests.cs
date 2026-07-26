using FluentAssertions;
using FluentValidation;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class PreBoardingServiceTests
{
    private readonly Mock<IFinalSelectionPoolRepository> _finalSelectionPoolRepositoryMock;
    private readonly Mock<IPreBoardingSubmissionRepository> _preBoardingSubmissionRepositoryMock;
    private readonly Mock<ICurrentCandidateService> _currentCandidateServiceMock;
    private readonly Mock<INotificationDispatchService> _notificationDispatchServiceMock;
    private readonly Mock<IApplicationSettingService> _applicationSettingServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PreBoardingService _service;

    private const long CandidateProfileId = 7;

    public PreBoardingServiceTests()
    {
        _finalSelectionPoolRepositoryMock = new Mock<IFinalSelectionPoolRepository>();
        _preBoardingSubmissionRepositoryMock = new Mock<IPreBoardingSubmissionRepository>();
        _currentCandidateServiceMock = new Mock<ICurrentCandidateService>();
        _currentCandidateServiceMock.Setup(c => c.GetOrCreateCurrentProfileIdAsync()).ReturnsAsync(CandidateProfileId);
        _notificationDispatchServiceMock = new Mock<INotificationDispatchService>();
        _applicationSettingServiceMock = new Mock<IApplicationSettingService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new PreBoardingService(
            _finalSelectionPoolRepositoryMock.Object,
            _preBoardingSubmissionRepositoryMock.Object,
            _currentCandidateServiceMock.Object,
            _notificationDispatchServiceMock.Object,
            _applicationSettingServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private static FinalSelectionPool OwnedPool(PreBoardingSubmission? submission = null) => new()
    {
        FinalSelectionPoolId = 1,
        JobApplicationId = 5,
        JobApplication = new JobApplication { JobApplicationId = 5, CandidateName = "John Smith" },
        PreBoardingSubmission = submission,
    };

    private static PreBoardingSubmission DraftSubmission() => new()
    {
        PreBoardingSubmissionId = 10,
        FinalSelectionPoolId = 1,
        Status = PreBoardingSubmissionStatusEnum.Draft,
    };

    private static PreBoardingSaveRequest CompleteRequest() => new()
    {
        EmergencyContactName = "Jane Smith",
        EmergencyContactRelationship = "Spouse",
        EmergencyContactPhone = "01700000000",
        BankName = "City Bank",
        BankAccountName = "John Smith",
        BankAccountNumber = "1234567890",
        Nominees = new List<PreBoardingNomineeRequest>
        {
            new() { FullName = "Jane Smith", Relationship = "Spouse", SharePercentage = 100m },
        },
    };

    [Fact]
    public async Task GetForCurrentCandidateAsync_NoPoolEntry_ShouldThrowNotFoundException()
    {
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByCandidateProfileIdWithDetailsAsync(CandidateProfileId))
            .ReturnsAsync((FinalSelectionPool?)null);

        var act = () => _service.GetForCurrentCandidateAsync();

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetForCurrentCandidateAsync_FirstTouch_ShouldAutoCreateDraftSubmission()
    {
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByCandidateProfileIdWithDetailsAsync(CandidateProfileId))
            .ReturnsAsync(OwnedPool());

        var response = await _service.GetForCurrentCandidateAsync();

        response.Status.Should().Be(PreBoardingSubmissionStatusEnum.Draft);
        _preBoardingSubmissionRepositoryMock.Verify(r => r.AddAsync(It.Is<PreBoardingSubmission>(
            s => s.FinalSelectionPoolId == 1 && s.Status == PreBoardingSubmissionStatusEnum.Draft)), Times.Once);
    }

    [Fact]
    public async Task SaveDraftAsync_WhenSubmitted_ShouldThrowInvalidStatusTransitionException()
    {
        var submitted = DraftSubmission();
        submitted.Status = PreBoardingSubmissionStatusEnum.Submitted;
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByCandidateProfileIdWithDetailsAsync(CandidateProfileId))
            .ReturnsAsync(OwnedPool(submitted));

        var act = () => _service.SaveDraftAsync(CompleteRequest());

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task SaveDraftAsync_Valid_ShouldReplaceNomineesCollection()
    {
        var submission = DraftSubmission();
        submission.Nominees.Add(new PreBoardingNominee { PreBoardingNomineeId = 1, FullName = "Old Nominee", SharePercentage = 100m });
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByCandidateProfileIdWithDetailsAsync(CandidateProfileId))
            .ReturnsAsync(OwnedPool(submission));

        var response = await _service.SaveDraftAsync(CompleteRequest());

        response.Nominees.Should().HaveCount(1);
        response.Nominees[0].FullName.Should().Be("Jane Smith");
        response.BankAccountNumber.Should().Be("1234567890");
    }

    [Fact]
    public async Task SubmitAsync_MissingRequiredFields_ShouldThrowValidationException()
    {
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByCandidateProfileIdWithDetailsAsync(CandidateProfileId))
            .ReturnsAsync(OwnedPool(DraftSubmission()));

        var act = () => _service.SubmitAsync();

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task SubmitAsync_NomineeSharesNotSumming100_ShouldThrowValidationException()
    {
        var submission = DraftSubmission();
        submission.EmergencyContactName = "Jane Smith";
        submission.EmergencyContactRelationship = "Spouse";
        submission.EmergencyContactPhone = "01700000000";
        submission.BankName = "City Bank";
        submission.BankAccountName = "John Smith";
        submission.BankAccountNumber = "1234567890";
        submission.Nominees.Add(new PreBoardingNominee { FullName = "Jane Smith", SharePercentage = 50m });
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByCandidateProfileIdWithDetailsAsync(CandidateProfileId))
            .ReturnsAsync(OwnedPool(submission));

        var act = () => _service.SubmitAsync();

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task SubmitAsync_Valid_ShouldSetSubmittedStatusAndDispatchHrNotification()
    {
        var submission = DraftSubmission();
        submission.EmergencyContactName = "Jane Smith";
        submission.EmergencyContactRelationship = "Spouse";
        submission.EmergencyContactPhone = "01700000000";
        submission.BankName = "City Bank";
        submission.BankAccountName = "John Smith";
        submission.BankAccountNumber = "1234567890";
        submission.Nominees.Add(new PreBoardingNominee { FullName = "Jane Smith", SharePercentage = 100m });
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByCandidateProfileIdWithDetailsAsync(CandidateProfileId))
            .ReturnsAsync(OwnedPool(submission));
        _applicationSettingServiceMock.Setup(s => s.GetHrNotificationEmailAsync()).ReturnsAsync("hr@example.com");

        var response = await _service.SubmitAsync();

        response.Status.Should().Be(PreBoardingSubmissionStatusEnum.Submitted);
        submission.SubmittedAt.Should().NotBeNull();
        _notificationDispatchServiceMock.Verify(n => n.DispatchAsync(
            RecruitmentEventEnum.PreBoardingSubmitted,
            It.IsAny<IDictionary<string, string>>(),
            It.IsAny<NotificationDispatchTargets>(),
            true,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
