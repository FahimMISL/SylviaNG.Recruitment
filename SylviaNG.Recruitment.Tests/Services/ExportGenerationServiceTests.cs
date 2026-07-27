using System.IO.Compression;
using System.Text;
using ClosedXML.Excel;
using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExportGenerationServiceTests
{
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly Mock<ICvPdfGeneratorService> _cvPdfGeneratorServiceMock;
    private readonly ExportGenerationService _service;

    public ExportGenerationServiceTests()
    {
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();
        _cvPdfGeneratorServiceMock = new Mock<ICvPdfGeneratorService>();
        _service = new ExportGenerationService(_jobApplicationRepositoryMock.Object, _candidateProfileRepositoryMock.Object, _cvPdfGeneratorServiceMock.Object);
    }

    private static JobPosting Posting() => new() { JobPostingId = 1, Title = "Software Engineer" };

    [Fact]
    public async Task GenerateCandidateListExportAsync_EmptyIds_ShouldReturnZeroRowsWithoutQuerying()
    {
        var result = await _service.GenerateCandidateListExportAsync(new List<long>(), ExportFormatEnum.Xlsx);

        result.RowCount.Should().Be(0);
        _jobApplicationRepositoryMock.Verify(r => r.Query(It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task GenerateCandidateListExportAsync_GuestApplicantWithNoProfile_ShouldUseSnapshotFields()
    {
        var applications = new List<JobApplication>
        {
            new()
            {
                JobApplicationId = 10,
                CandidateProfileId = null,
                CandidateName = "Guest Applicant",
                CandidateEmail = "guest@example.com",
                CandidatePhone = "0100000000",
                JobPosting = Posting(),
                ApplicationStatus = ApplicationStatusEnum.Applied,
                Source = ApplicationSourceEnum.External,
                AppliedDate = new DateTime(2026, 7, 1)
            }
        };
        _jobApplicationRepositoryMock.Setup(r => r.Query(It.IsAny<bool>())).Returns(applications.AsQueryable());

        var result = await _service.GenerateCandidateListExportAsync(new List<long> { 10 }, ExportFormatEnum.Xlsx);

        result.RowCount.Should().Be(1);
        _candidateProfileRepositoryMock.Verify(r => r.GetByIdsWithDetailsAsync(It.IsAny<IEnumerable<long>>()), Times.Never);

        using var workbook = new XLWorkbook(new MemoryStream(result.Content));
        var sheet = workbook.Worksheet(1);
        sheet.Cell(2, 1).GetString().Should().Be("Guest Applicant");
        sheet.Cell(2, 2).GetString().Should().Be("guest@example.com");
        sheet.Cell(2, 4).GetString().Should().Be("Software Engineer");
    }

    [Fact]
    public async Task GenerateCandidateListExportAsync_ApplicantWithProfile_ShouldPreferProfileFields()
    {
        var applications = new List<JobApplication>
        {
            new()
            {
                JobApplicationId = 20,
                CandidateProfileId = 5,
                CandidateName = "Snapshot Name",
                CandidateEmail = "snapshot@example.com",
                JobPosting = Posting(),
                ApplicationStatus = ApplicationStatusEnum.Shortlisted,
                Source = ApplicationSourceEnum.Internal,
                AppliedDate = new DateTime(2026, 7, 2)
            }
        };
        _jobApplicationRepositoryMock.Setup(r => r.Query(It.IsAny<bool>())).Returns(applications.AsQueryable());

        var profile = new CandidateProfile
        {
            CandidateProfileId = 5,
            FullName = "Profile Name",
            Email = "profile@example.com",
            Skills = new List<CandidateSkill> { new() { SkillName = "C#" }, new() { SkillName = "SQL" } }
        };
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdsWithDetailsAsync(It.Is<IEnumerable<long>>(ids => ids.Contains(5))))
            .ReturnsAsync(new List<CandidateProfile> { profile });

        var result = await _service.GenerateCandidateListExportAsync(new List<long> { 20 }, ExportFormatEnum.Csv);

        result.RowCount.Should().Be(1);
        result.ContentType.Should().Be("text/csv");
        var csv = System.Text.Encoding.UTF8.GetString(result.Content);
        csv.Should().Contain("Profile Name");
        csv.Should().Contain("profile@example.com");
        csv.Should().Contain("C#, SQL");
    }

    [Fact]
    public async Task GenerateBulkCvZipAsync_ApplicationWithProfile_ShouldIncludeOneZipEntry()
    {
        var applications = new List<JobApplication>
        {
            new() { JobApplicationId = 30, CandidateProfileId = 7, CandidateName = "Jane Doe" }
        };
        _jobApplicationRepositoryMock.Setup(r => r.Query(It.IsAny<bool>())).Returns(applications.AsQueryable());

        var profile = new CandidateProfile { CandidateProfileId = 7, FullName = "Jane Doe" };
        _candidateProfileRepositoryMock.Setup(r => r.GetByIdsWithDetailsAsync(It.Is<IEnumerable<long>>(ids => ids.Contains(7))))
            .ReturnsAsync(new List<CandidateProfile> { profile });
        _cvPdfGeneratorServiceMock.Setup(g => g.Generate(profile)).Returns(Encoding.UTF8.GetBytes("pdf-bytes"));

        var result = await _service.GenerateBulkCvZipAsync(new List<long> { 30 });

        result.RowCount.Should().Be(1);
        result.ContentType.Should().Be("application/zip");
        using var archive = new ZipArchive(new MemoryStream(result.Content), ZipArchiveMode.Read);
        archive.Entries.Should().ContainSingle(e => e.Name == "Jane_Doe_30.pdf");
    }

    [Fact]
    public async Task GenerateBulkCvZipAsync_GuestApplicantWithNoProfile_ShouldBeSkipped()
    {
        var applications = new List<JobApplication>
        {
            new() { JobApplicationId = 31, CandidateProfileId = null, CandidateName = "Guest Applicant" }
        };
        _jobApplicationRepositoryMock.Setup(r => r.Query(It.IsAny<bool>())).Returns(applications.AsQueryable());

        var result = await _service.GenerateBulkCvZipAsync(new List<long> { 31 });

        result.RowCount.Should().Be(0);
        using var archive = new ZipArchive(new MemoryStream(result.Content), ZipArchiveMode.Read);
        archive.Entries.Should().BeEmpty();
        _cvPdfGeneratorServiceMock.Verify(g => g.Generate(It.IsAny<CandidateProfile>()), Times.Never);
    }
}
