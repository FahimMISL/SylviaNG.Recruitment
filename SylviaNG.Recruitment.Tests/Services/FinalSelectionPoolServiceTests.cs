using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class FinalSelectionPoolServiceTests
{
    private readonly Mock<IFinalSelectionPoolRepository> _finalSelectionPoolRepositoryMock;
    private readonly Mock<INotificationDispatchService> _notificationDispatchServiceMock;
    private readonly Mock<IApplicationSettingService> _applicationSettingServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly FinalSelectionPoolService _service;

    public FinalSelectionPoolServiceTests()
    {
        _finalSelectionPoolRepositoryMock = new Mock<IFinalSelectionPoolRepository>();
        _notificationDispatchServiceMock = new Mock<INotificationDispatchService>();
        _applicationSettingServiceMock = new Mock<IApplicationSettingService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _applicationSettingServiceMock.Setup(s => s.GetHrNotificationEmailAsync()).ReturnsAsync((string?)null);

        _service = new FinalSelectionPoolService(
            _finalSelectionPoolRepositoryMock.Object,
            _notificationDispatchServiceMock.Object,
            _applicationSettingServiceMock.Object,
            Options.Create(new PortalSettings()),
            _unitOfWorkMock.Object);
    }

    private static OfferLetter AcceptedOfferLetter() => new()
    {
        OfferLetterId = 1,
        JobApplicationId = 5,
        Designation = "Software Engineer",
        JoiningDate = new DateTime(2026, 8, 1),
        Status = OfferLetterStatusEnum.Accepted,
        JobApplication = new JobApplication { JobApplicationId = 5, CandidateName = "John Smith", CandidateEmail = "john@example.com" },
    };

    [Fact]
    public async Task CreateFromAcceptedOfferAsync_ValidOffer_ShouldCreatePoolEntryAndDispatchNotification()
    {
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByOfferLetterIdAsync(1)).ReturnsAsync((FinalSelectionPool?)null);

        await _service.CreateFromAcceptedOfferAsync(AcceptedOfferLetter());

        _finalSelectionPoolRepositoryMock.Verify(r => r.AddAsync(It.Is<FinalSelectionPool>(
            p => p.OfferLetterId == 1 && p.JobApplicationId == 5 && p.JoiningDate == new DateTime(2026, 8, 1) && !p.HasJoined)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _notificationDispatchServiceMock.Verify(n => n.DispatchAsync(
            RecruitmentEventEnum.PreBoardingRequested,
            It.IsAny<IDictionary<string, string>>(),
            It.IsAny<NotificationDispatchTargets>(),
            true,
            It.IsAny<IReadOnlyList<EmailAttachment>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateFromAcceptedOfferAsync_AlreadyInPool_ShouldNotCreateDuplicate()
    {
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByOfferLetterIdAsync(1))
            .ReturnsAsync(new FinalSelectionPool { FinalSelectionPoolId = 9, OfferLetterId = 1 });

        await _service.CreateFromAcceptedOfferAsync(AcceptedOfferLetter());

        _finalSelectionPoolRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FinalSelectionPool>()), Times.Never);
        _notificationDispatchServiceMock.Verify(n => n.DispatchAsync(
            It.IsAny<RecruitmentEventEnum>(),
            It.IsAny<IDictionary<string, string>>(),
            It.IsAny<NotificationDispatchTargets>(),
            It.IsAny<bool>(),
            It.IsAny<IReadOnlyList<EmailAttachment>>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ShouldThrowNotFoundException()
    {
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(99)).ReturnsAsync((FinalSelectionPool?)null);

        var act = () => _service.GetByIdAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task MarkHasJoinedAsync_AlreadyJoined_ShouldThrowInvalidStatusTransitionException()
    {
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(new FinalSelectionPool { FinalSelectionPoolId = 1, HasJoined = true });

        var act = () => _service.MarkHasJoinedAsync(1);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task MarkHasJoinedAsync_Valid_ShouldSetHasJoinedAndJoinedAt()
    {
        var entity = new FinalSelectionPool { FinalSelectionPoolId = 1, HasJoined = false };
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(entity);

        var response = await _service.MarkHasJoinedAsync(1);

        response.HasJoined.Should().BeTrue();
        entity.JoinedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateBatchAsync_NotFound_ShouldThrowNotFoundException()
    {
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(99)).ReturnsAsync((FinalSelectionPool?)null);

        var act = () => _service.UpdateBatchAsync(99, new FinalSelectionPoolUpdateBatchRequest { BatchLabel = "Batch 1", JoiningDate = DateTime.UtcNow });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateBatchAsync_Valid_ShouldSetBatchLabelAndJoiningDate()
    {
        var entity = new FinalSelectionPool { FinalSelectionPoolId = 1 };
        _finalSelectionPoolRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(entity);
        var joiningDate = new DateTime(2026, 9, 1);

        var response = await _service.UpdateBatchAsync(1, new FinalSelectionPoolUpdateBatchRequest { BatchLabel = "Batch 2", JoiningDate = joiningDate });

        response.BatchLabel.Should().Be("Batch 2");
        response.JoiningDate.Should().Be(joiningDate);
    }
}
