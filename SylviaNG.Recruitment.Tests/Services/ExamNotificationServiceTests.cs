using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExamNotificationServiceTests
{
    private readonly Mock<IExamEnrollmentRepository> _examEnrollmentRepositoryMock;
    private readonly Mock<ISmtpEmailService> _smtpEmailServiceMock;
    private readonly Mock<ISmsNotificationService> _smsNotificationServiceMock;
    private readonly Mock<IAdmitCardPdfGeneratorService> _admitCardPdfGeneratorServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExamNotificationService _service;

    public ExamNotificationServiceTests()
    {
        _examEnrollmentRepositoryMock = new Mock<IExamEnrollmentRepository>();
        _smtpEmailServiceMock = new Mock<ISmtpEmailService>();
        _smsNotificationServiceMock = new Mock<ISmsNotificationService>();
        _admitCardPdfGeneratorServiceMock = new Mock<IAdmitCardPdfGeneratorService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _admitCardPdfGeneratorServiceMock
            .Setup(g => g.Generate(It.IsAny<ExamEnrollment>(), It.IsAny<Exam>(), It.IsAny<JobApplication>()))
            .Returns(new byte[] { 1, 2, 3 });

        _service = new ExamNotificationService(
            _examEnrollmentRepositoryMock.Object,
            _smtpEmailServiceMock.Object,
            _smsNotificationServiceMock.Object,
            _admitCardPdfGeneratorServiceMock.Object,
            Options.Create(new PortalSettings { FrontendBaseUrl = "http://localhost:4600" }),
            _unitOfWorkMock.Object,
            new Mock<ILogger<ExamNotificationService>>().Object);
    }

    private static Exam ExamFor(long examId) => new() { ExamId = examId, Title = "Written Test" };

    private static ExamEnrollment EnrollmentFor(long id, long examId, Exam exam, string? email = "candidate@example.com", string? phone = "01700000000") => new()
    {
        ExamEnrollmentId = id,
        ExamId = examId,
        JobApplicationId = id,
        Exam = exam,
        JobApplication = new JobApplication
        {
            JobApplicationId = id,
            CandidateName = "Candidate " + id,
            CandidateEmail = email,
            CandidatePhone = phone,
        },
    };

    [Fact]
    public async Task DistributeBulkAsync_ShouldNotifyEveryEnrollmentAndReturnSentCounts()
    {
        var exam = ExamFor(1);
        var enrollments = new List<ExamEnrollment>
        {
            EnrollmentFor(1, 1, exam),
            EnrollmentFor(2, 1, exam),
        };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdWithDetailsAsync(1)).ReturnsAsync(enrollments);

        _smtpEmailServiceMock
            .Setup(s => s.TrySendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult { Success = true });
        _smsNotificationServiceMock
            .Setup(s => s.TrySendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var (emailSentCount, smsSentCount, totalCount) = await _service.DistributeBulkAsync(1);

        totalCount.Should().Be(2);
        emailSentCount.Should().Be(2);
        smsSentCount.Should().Be(2);
        enrollments.Should().OnlyContain(e => e.EmailNotificationStatus == NotificationStatusEnum.Sent);
        enrollments.Should().OnlyContain(e => e.SmsNotificationStatus == NotificationStatusEnum.Sent);
        _examEnrollmentRepositoryMock.Verify(r => r.Update(It.IsAny<ExamEnrollment>()), Times.Exactly(2));
    }

    [Fact]
    public async Task DistributeBulkAsync_SmsBodyShouldIncludePortalDownloadLink()
    {
        var exam = ExamFor(1);
        var enrollments = new List<ExamEnrollment> { EnrollmentFor(1, 1, exam) };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdWithDetailsAsync(1)).ReturnsAsync(enrollments);

        _smtpEmailServiceMock
            .Setup(s => s.TrySendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult { Success = true });

        string? capturedMessage = null;
        _smsNotificationServiceMock
            .Setup(s => s.TrySendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, string, CancellationToken>((_, message, _) => capturedMessage = message)
            .ReturnsAsync(true);

        await _service.DistributeBulkAsync(1);

        capturedMessage.Should().Contain("http://localhost:4600/my-applications");
    }

    [Fact]
    public async Task DistributeBulkAsync_CandidateWithNoEmailOrPhone_ShouldMarkChannelsSkippedNotFailed()
    {
        var exam = ExamFor(1);
        var enrollments = new List<ExamEnrollment> { EnrollmentFor(1, 1, exam, email: null, phone: null) };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdWithDetailsAsync(1)).ReturnsAsync(enrollments);

        var (emailSentCount, smsSentCount, totalCount) = await _service.DistributeBulkAsync(1);

        totalCount.Should().Be(1);
        emailSentCount.Should().Be(0);
        smsSentCount.Should().Be(0);
        enrollments[0].EmailNotificationStatus.Should().Be(NotificationStatusEnum.Skipped);
        enrollments[0].SmsNotificationStatus.Should().Be(NotificationStatusEnum.Skipped);
        _smtpEmailServiceMock.Verify(s => s.TrySendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
