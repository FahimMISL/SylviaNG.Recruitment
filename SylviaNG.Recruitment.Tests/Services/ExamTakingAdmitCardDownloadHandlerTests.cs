using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Queries.ExamTakingAdmitCardDownload;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExamTakingAdmitCardDownloadHandlerTests
{
    private readonly Mock<IExamEnrollmentRepository> _examEnrollmentRepositoryMock;
    private readonly Mock<ICurrentCandidateService> _currentCandidateServiceMock;
    private readonly Mock<IAdmitCardPdfGeneratorService> _admitCardPdfGeneratorServiceMock;
    private readonly ExamTakingAdmitCardDownloadHandler _handler;

    public ExamTakingAdmitCardDownloadHandlerTests()
    {
        _examEnrollmentRepositoryMock = new Mock<IExamEnrollmentRepository>();
        _currentCandidateServiceMock = new Mock<ICurrentCandidateService>();
        _admitCardPdfGeneratorServiceMock = new Mock<IAdmitCardPdfGeneratorService>();

        _handler = new ExamTakingAdmitCardDownloadHandler(
            _examEnrollmentRepositoryMock.Object,
            _currentCandidateServiceMock.Object,
            _admitCardPdfGeneratorServiceMock.Object);
    }

    private static ExamEnrollment EnrollmentOwnedBy(long candidateProfileId) => new()
    {
        ExamEnrollmentId = 1,
        ExamId = 1,
        Exam = new Exam { ExamId = 1, Title = "Written Test" },
        JobApplication = new JobApplication
        {
            JobApplicationId = 1,
            CandidateProfileId = candidateProfileId,
            CandidateName = "Owner",
        },
    };

    [Fact]
    public async Task Handle_OwnEnrollment_ShouldReturnPdf()
    {
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(EnrollmentOwnedBy(5));
        _currentCandidateServiceMock.Setup(s => s.GetOrCreateCurrentProfileIdAsync()).ReturnsAsync(5);
        _admitCardPdfGeneratorServiceMock
            .Setup(g => g.Generate(It.IsAny<ExamEnrollment>(), It.IsAny<Exam>(), It.IsAny<JobApplication>()))
            .Returns(new byte[] { 1, 2, 3 });

        var result = await _handler.Handle(new ExamTakingAdmitCardDownloadQuery(1), CancellationToken.None);

        result.Content.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        result.ContentType.Should().Be("application/pdf");
    }

    [Fact]
    public async Task Handle_AnotherCandidatesEnrollment_ShouldThrowForbiddenException()
    {
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(EnrollmentOwnedBy(5));
        _currentCandidateServiceMock.Setup(s => s.GetOrCreateCurrentProfileIdAsync()).ReturnsAsync(999);

        var act = () => _handler.Handle(new ExamTakingAdmitCardDownloadQuery(1), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_UnknownEnrollment_ShouldThrowNotFoundException()
    {
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync((ExamEnrollment?)null);

        var act = () => _handler.Handle(new ExamTakingAdmitCardDownloadQuery(1), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
