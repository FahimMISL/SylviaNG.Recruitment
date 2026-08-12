using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionBulkImport;

namespace SylviaNG.Recruitment.Tests.Validators;

public class ExamQuestionBulkImportValidatorTests
{
    private readonly ExamQuestionBulkImportValidator _validator = new(Options.Create(new ExamQuestionImportSettings()));

    // Leading bytes of a real .xlsx (OOXML = zip container, "PK\x03\x04"). The validator now
    // magic-byte checks content against the extension (FileSignatureValidator), so a bare mock
    // with no readable stream would NRE / fail the signature rule. A fresh stream is returned per
    // call since the validator opens-and-disposes the stream once per rule.
    private static readonly byte[] XlsxSignatureBytes = { 0x50, 0x4B, 0x03, 0x04, 0x00, 0x00, 0x00, 0x00 };

    private static IFormFile CreateFile(string fileName, long length)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(XlsxSignatureBytes));
        return fileMock.Object;
    }

    [Fact]
    public void Validate_WithValidXlsxFile_ShouldHaveNoErrors()
    {
        var command = new ExamQuestionBulkImportCommand(1, CreateFile("questions.xlsx", 1024));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NoFile_ShouldHaveValidationError()
    {
        var command = new ExamQuestionBulkImportCommand(1, null!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("File is required"));
    }

    [Fact]
    public void Validate_UnsupportedExtension_ShouldHaveValidationError()
    {
        var command = new ExamQuestionBulkImportCommand(1, CreateFile("questions.pdf", 1024));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("File extension"));
    }

    [Fact]
    public void Validate_FileTooLarge_ShouldHaveValidationError()
    {
        var tooLarge = new ExamQuestionImportSettings().MaxFileSizeMB * 1024L * 1024L + 1;
        var command = new ExamQuestionBulkImportCommand(1, CreateFile("questions.xlsx", tooLarge));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("File size"));
    }

    [Fact]
    public void Validate_MissingQuestionGroupId_ShouldHaveValidationError()
    {
        var command = new ExamQuestionBulkImportCommand(0, CreateFile("questions.xlsx", 1024));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "QuestionGroupId");
    }
}
