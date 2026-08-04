using FluentAssertions;
using FluentValidation;
using Moq;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class NotificationLogServiceTests
{
    private readonly Mock<INotificationLogRepository> _repositoryMock;
    private readonly Mock<ISmtpEmailService> _smtpEmailServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentCandidateService> _currentCandidateServiceMock;
    private readonly NotificationLogService _service;

    public NotificationLogServiceTests()
    {
        _repositoryMock = new Mock<INotificationLogRepository>();
        _smtpEmailServiceMock = new Mock<ISmtpEmailService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _currentCandidateServiceMock = new Mock<ICurrentCandidateService>();

        _service = new NotificationLogService(_repositoryMock.Object, _smtpEmailServiceMock.Object, _unitOfWorkMock.Object, _currentCandidateServiceMock.Object);
    }

    [Fact]
    public async Task RetryAsync_UnknownId_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((NotificationLog?)null);

        var act = () => _service.RetryAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task RetryAsync_NotFailed_ShouldThrowValidationException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NotificationLog
        {
            NotificationLogId = 1,
            DeliveryStatus = NotificationStatusEnum.Sent,
        });

        var act = () => _service.RetryAsync(1);

        await act.Should().ThrowAsync<ValidationException>();
        _smtpEmailServiceMock.Verify(s => s.TrySendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RetryAsync_FailedAndResendSucceeds_ShouldFlipToSentAndClearFailureReason()
    {
        var entity = new NotificationLog
        {
            NotificationLogId = 1,
            DeliveryStatus = NotificationStatusEnum.Failed,
            FailureReason = "SMTP timeout",
            RecipientAddress = "candidate@example.com",
            RenderedSubject = "Subject",
            RenderedBody = "Body",
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _smtpEmailServiceMock.Setup(s => s.TrySendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult { Success = true });

        var result = await _service.RetryAsync(1);

        result.DeliveryStatus.Should().Be(NotificationStatusEnum.Sent);
        entity.FailureReason.Should().BeNull();
        entity.SentAt.Should().NotBeNull();
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RetryAsync_FailedAndResendFailsAgain_ShouldStayFailedWithNewReason()
    {
        var entity = new NotificationLog
        {
            NotificationLogId = 1,
            DeliveryStatus = NotificationStatusEnum.Failed,
            RecipientAddress = "candidate@example.com",
            RenderedSubject = "Subject",
            RenderedBody = "Body",
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _smtpEmailServiceMock.Setup(s => s.TrySendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult { Success = false, ErrorMessage = "still down" });

        var result = await _service.RetryAsync(1);

        result.DeliveryStatus.Should().Be(NotificationStatusEnum.Failed);
        result.FailureReason.Should().Be("still down");
    }

    [Fact]
    public async Task MarkAsReadAsync_UnknownId_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((NotificationLog?)null);

        var act = () => _service.MarkAsReadAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task MarkAsReadAsync_AlreadyRead_ShouldBeNoOp()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new NotificationLog { NotificationLogId = 1, IsRead = true });

        await _service.MarkAsReadAsync(1);

        _repositoryMock.Verify(r => r.Update(It.IsAny<NotificationLog>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task MarkAsReadAsync_Unread_ShouldSetIsReadAndSave()
    {
        var entity = new NotificationLog { NotificationLogId = 1, IsRead = false };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        await _service.MarkAsReadAsync(1);

        entity.IsRead.Should().BeTrue();
        entity.ReadAt.Should().NotBeNull();
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task MarkAllAsReadAsync_ShouldDelegateToRepositoryAndReturnCount()
    {
        _repositoryMock.Setup(r => r.MarkAllAsReadForAdminHrAsync()).ReturnsAsync(3);

        var result = await _service.MarkAllAsReadAsync();

        result.Should().Be(3);
    }

    [Fact]
    public async Task GetUnreadCountAsync_ShouldDelegateToRepository()
    {
        _repositoryMock.Setup(r => r.GetUnreadCountForAdminHrAsync()).ReturnsAsync(5);

        var result = await _service.GetUnreadCountAsync();

        result.Should().Be(5);
    }

    [Fact]
    public async Task GetUnreadAsync_ShouldMapEntitiesToResponses()
    {
        _repositoryMock.Setup(r => r.GetUnreadForAdminHrAsync(10)).ReturnsAsync(new List<NotificationLog>
        {
            new()
            {
                NotificationLogId = 1,
                RecipientType = NotificationRecipientTypeEnum.AdminHr,
                RecruitmentEvent = RecruitmentEventEnum.ApplicationSubmitted,
                RenderedSubject = "New application",
            },
        });

        var result = await _service.GetUnreadAsync();

        result.Should().HaveCount(1);
        result[0].RecipientName.Should().Be("Admin / HR");
        result[0].RenderedSubject.Should().Be("New application");
    }
}
