using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExamQuestionImportServiceTests
{
    private readonly Mock<IExamQuestionRepository> _examQuestionRepositoryMock;
    private readonly Mock<IQuestionGroupRepository> _questionGroupRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExamQuestionImportService _service;

    public ExamQuestionImportServiceTests()
    {
        _examQuestionRepositoryMock = new Mock<IExamQuestionRepository>();
        _questionGroupRepositoryMock = new Mock<IQuestionGroupRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _questionGroupRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new QuestionGroup { QuestionGroupId = 1, Name = "Aptitude" });

        _service = new ExamQuestionImportService(_examQuestionRepositoryMock.Object, _questionGroupRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private const string Header = "QuestionText,QuestionType,DifficultyLevel,Marks,Option1Text,Option1IsCorrect,Option2Text,Option2IsCorrect,Option3Text,Option3IsCorrect,Option4Text,Option4IsCorrect,Explanation,ModelAnswer";

    private static IFormFile CreateCsvFile(params string[] dataRows)
    {
        var content = string.Join("\n", new[] { Header }.Concat(dataRows));
        var bytes = Encoding.UTF8.GetBytes(content);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("questions.csv");
        fileMock.Setup(f => f.Length).Returns(bytes.Length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(bytes));

        return fileMock.Object;
    }

    [Fact]
    public async Task ImportAsync_AllValidRows_ShouldImportAllAndReturnZeroErrors()
    {
        var file = CreateCsvFile(
            "What is 2+2?,McqSingle,Easy,5,3,FALSE,4,TRUE,,,,,Basic math,",
            "The sky is blue.,TrueFalse,Easy,2,True,TRUE,False,FALSE,,,,,,",
            "Explain OOP.,Subjective,Medium,10,,,,,,,,,,Encapsulation inheritance polymorphism");

        var result = await _service.ImportAsync(1, file);

        result.TotalRows.Should().Be(3);
        result.ImportedCount.Should().Be(3);
        result.FailedCount.Should().Be(0);
        result.Errors.Should().BeEmpty();
        _examQuestionRepositoryMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<ExamQuestion>>(qs => qs.Count() == 3)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ImportAsync_RowWithMissingQuestionText_ShouldSkipRowAndReportError()
    {
        var file = CreateCsvFile(",McqSingle,Easy,5,3,FALSE,4,TRUE,,,,,,");

        var result = await _service.ImportAsync(1, file);

        result.TotalRows.Should().Be(1);
        result.ImportedCount.Should().Be(0);
        result.FailedCount.Should().Be(1);
        result.Errors.Should().ContainSingle();
        result.Errors[0].RowNumber.Should().Be(2);
        result.Errors[0].Message.Should().Contain("QuestionText");
    }

    [Fact]
    public async Task ImportAsync_McqRowWithNoCorrectOption_ShouldSkipRowAndReportError()
    {
        var file = CreateCsvFile("What is 2+2?,McqSingle,Easy,5,3,FALSE,4,FALSE,,,,,,");

        var result = await _service.ImportAsync(1, file);

        result.ImportedCount.Should().Be(0);
        result.FailedCount.Should().Be(1);
        result.Errors[0].Message.Should().Contain("exactly 1 correct option");
    }

    [Fact]
    public async Task ImportAsync_MixOfValidAndInvalidRows_ShouldImportOnlyValidAndReportSummaryCounts()
    {
        var file = CreateCsvFile(
            "What is 2+2?,McqSingle,Easy,5,3,FALSE,4,TRUE,,,,,,",
            ",McqSingle,Easy,5,3,FALSE,4,TRUE,,,,,,",
            "Explain OOP.,Subjective,Medium,10,,,,,,,,,,Model answer text");

        var result = await _service.ImportAsync(1, file);

        result.TotalRows.Should().Be(3);
        result.ImportedCount.Should().Be(2);
        result.FailedCount.Should().Be(1);
        result.Errors.Should().ContainSingle(e => e.RowNumber == 3);
    }

    [Fact]
    public async Task ImportAsync_WithUnknownQuestionGroupId_ShouldThrowNotFoundException()
    {
        _questionGroupRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((QuestionGroup?)null);
        var file = CreateCsvFile("What is 2+2?,McqSingle,Easy,5,3,FALSE,4,TRUE,,,,,,");

        var act = () => _service.ImportAsync(99, file);

        await act.Should().ThrowAsync<NotFoundException>();
        _examQuestionRepositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<ExamQuestion>>()), Times.Never);
    }

    [Fact]
    public void GenerateTemplate_ShouldProduceParsableXlsxWithExpectedHeaders()
    {
        var template = _service.GenerateTemplate();

        template.Content.Should().NotBeEmpty();
        template.FileName.Should().Be("ExamQuestionImportTemplate.xlsx");

        using var stream = new MemoryStream(template.Content);
        using var document = SpreadsheetDocument.Open(stream, false);
        var worksheetPart = (WorksheetPart)document.WorkbookPart!.GetPartById(
            document.WorkbookPart.Workbook!.Descendants<Sheet>().First().Id!.Value!);
        var headerRow = worksheetPart.Worksheet!.Descendants<Row>().First();
        var headerTexts = headerRow.Elements<Cell>().Select(c => c.InlineString?.Text?.Text).ToList();

        headerTexts.Should().Contain("QuestionText");
        headerTexts.Should().Contain("QuestionType");
        headerTexts.Should().Contain("Option1Text");
        headerTexts.Should().Contain("ModelAnswer");
    }
}
