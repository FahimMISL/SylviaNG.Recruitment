using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExamScoreImportServiceTests
{
    private readonly Mock<IExamRepository> _examRepositoryMock;
    private readonly Mock<IExamEnrollmentRepository> _examEnrollmentRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExamScoreImportService _service;

    public ExamScoreImportServiceTests()
    {
        _examRepositoryMock = new Mock<IExamRepository>();
        _examEnrollmentRepositoryMock = new Mock<IExamEnrollmentRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _examRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Exam { ExamId = 1, TotalMarks = 100, PassMarks = 40 });
        _currentUserServiceMock.Setup(s => s.GetCurrentUserName()).Returns("hr.abir");

        _service = new ExamScoreImportService(
            _examRepositoryMock.Object,
            _examEnrollmentRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private const string Header = "ExamEnrollmentId,CandidateName,Score";

    private static IFormFile CreateCsvFile(params string[] dataRows)
    {
        var content = string.Join("\n", new[] { Header }.Concat(dataRows));
        var bytes = Encoding.UTF8.GetBytes(content);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("scores.csv");
        fileMock.Setup(f => f.Length).Returns(bytes.Length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(bytes));

        return fileMock.Object;
    }

    private static List<ExamEnrollment> EnrollmentsFor(params long[] ids) =>
        ids.Select(id => new ExamEnrollment
        {
            ExamEnrollmentId = id,
            ExamId = 1,
            JobApplication = new JobApplication { CandidateName = "Candidate " + id },
        }).ToList();

    [Fact]
    public async Task ImportAsync_ValidRow_ShouldUpdateEnrollmentScoreAndPassStatus()
    {
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(EnrollmentsFor(10));
        var file = CreateCsvFile("10,Candidate 10,45");

        var result = await _service.ImportAsync(1, file);

        result.TotalRows.Should().Be(1);
        result.UpdatedCount.Should().Be(1);
        result.FailedCount.Should().Be(0);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ImportAsync_ScoreAboveTotalMarks_ShouldReportRowError()
    {
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(EnrollmentsFor(10));
        var file = CreateCsvFile("10,Candidate 10,150");

        var result = await _service.ImportAsync(1, file);

        result.UpdatedCount.Should().Be(0);
        result.FailedCount.Should().Be(1);
        result.Errors[0].Message.Should().Contain("between 0 and");
    }

    [Fact]
    public async Task ImportAsync_UnknownExamEnrollmentId_ShouldReportRowError()
    {
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(EnrollmentsFor(10));
        var file = CreateCsvFile("999,Someone,50");

        var result = await _service.ImportAsync(1, file);

        result.FailedCount.Should().Be(1);
        result.Errors[0].Message.Should().Contain("does not belong to this exam");
    }

    [Fact]
    public async Task ImportAsync_NonNumericScore_ShouldReportRowError()
    {
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(EnrollmentsFor(10));
        var file = CreateCsvFile("10,Candidate 10,not-a-number");

        var result = await _service.ImportAsync(1, file);

        result.FailedCount.Should().Be(1);
    }

    [Fact]
    public async Task ImportAsync_MixOfValidAndInvalidRows_ShouldUpdateOnlyValidRows()
    {
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(EnrollmentsFor(10, 11));
        var file = CreateCsvFile("10,Candidate 10,45", "11,Candidate 11,200");

        var result = await _service.ImportAsync(1, file);

        result.TotalRows.Should().Be(2);
        result.UpdatedCount.Should().Be(1);
        result.FailedCount.Should().Be(1);
    }

    [Fact]
    public async Task ImportAsync_WithUnknownExamId_ShouldThrowNotFoundException()
    {
        _examRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Exam?)null);
        var file = CreateCsvFile("10,Candidate 10,45");

        var act = () => _service.ImportAsync(99, file);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ImportAsync_ValidRow_ShouldStampManualUploadSourceAndUploaderIdentity()
    {
        var enrollment = EnrollmentsFor(10).Single();
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(new List<ExamEnrollment> { enrollment });
        var file = CreateCsvFile("10,Candidate 10,45");

        await _service.ImportAsync(1, file);

        enrollment.ScoreSource.Should().Be(ScoreSourceEnum.ManualUpload);
        enrollment.ScoredByUserName.Should().Be("hr.abir");
        enrollment.IsPassed.Should().BeTrue();
    }

    [Fact]
    public async Task GenerateTemplateAsync_ShouldProduceParsableXlsxPrefilledWithEnrollments()
    {
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(EnrollmentsFor(10));

        var template = await _service.GenerateTemplateAsync(1);

        template.Content.Should().NotBeEmpty();
        template.FileName.Should().Be("ExamScoreUploadTemplate-1.xlsx");

        using var stream = new MemoryStream(template.Content);
        using var document = SpreadsheetDocument.Open(stream, false);
        var worksheetPart = (WorksheetPart)document.WorkbookPart!.GetPartById(
            document.WorkbookPart.Workbook!.Descendants<Sheet>().First().Id!.Value!);
        var rows = worksheetPart.Worksheet!.Descendants<Row>().ToList();
        var headerTexts = rows[0].Elements<Cell>().Select(c => c.InlineString?.Text?.Text).ToList();

        headerTexts.Should().Contain("ExamEnrollmentId");
        headerTexts.Should().Contain("Score");
        rows.Should().HaveCount(2); // header + 1 enrollment row
    }
}
