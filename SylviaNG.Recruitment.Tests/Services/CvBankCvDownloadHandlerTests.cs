using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankCvDownload;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Tests.Services;

public class CvBankCvDownloadHandlerTests
{
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<ICvPdfGeneratorService> _cvPdfGeneratorServiceMock;
    private readonly CvBankCvDownloadHandler _handler;

    public CvBankCvDownloadHandlerTests()
    {
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _cvPdfGeneratorServiceMock = new Mock<ICvPdfGeneratorService>();
        _handler = new CvBankCvDownloadHandler(
            _candidateProfileRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object,
            _cvPdfGeneratorServiceMock.Object);
    }

    [Fact]
    public async Task Handle_CandidateWithNoApplicationInCallerCompany_ShouldThrowNotFoundException()
    {
        // Security regression (2026-08-16): CandidateProfile carries no CompanyId of its own
        // (candidates apply across companies) - a candidate must not be downloadable by a
        // company they never applied to, mirroring the check CvBankSearchHandler already applies.
        var profile = new CandidateProfile { CandidateProfileId = 42, FullName = "Jane Doe" };
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdsWithDetailsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<CandidateProfile> { profile });
        _jobApplicationRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, bool>>>()))
            .ReturnsAsync(new List<JobApplication>());

        var act = () => _handler.Handle(new CvBankCvDownloadQuery(42), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        _cvPdfGeneratorServiceMock.Verify(s => s.Generate(It.IsAny<CandidateProfile>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CandidateWithApplicationInCallerCompany_ShouldGenerateCv()
    {
        var profile = new CandidateProfile { CandidateProfileId = 42, FullName = "Jane Doe" };
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdsWithDetailsAsync(It.IsAny<IEnumerable<long>>()))
            .ReturnsAsync(new List<CandidateProfile> { profile });
        _jobApplicationRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<JobApplication, bool>>>()))
            .ReturnsAsync(new List<JobApplication> { new() { CandidateProfileId = 42 } });
        _cvPdfGeneratorServiceMock.Setup(s => s.Generate(profile)).ReturnsAsync(new byte[] { 1, 2, 3 });

        var response = await _handler.Handle(new CvBankCvDownloadQuery(42), CancellationToken.None);

        response.Content.Should().Equal(new byte[] { 1, 2, 3 });
    }
}
