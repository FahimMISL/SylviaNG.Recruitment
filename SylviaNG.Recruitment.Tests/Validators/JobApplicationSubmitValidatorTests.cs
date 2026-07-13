using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobApplicationSubmit;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Validators;

public class JobApplicationSubmitValidatorTests
{
    private readonly JobApplicationSubmitValidator _validator;

    public JobApplicationSubmitValidatorTests()
    {
        var settings = new ApplicationCvStorageSettings
        {
            RootPath = "wwwroot/uploads/applications",
            MaxFileSizeMB = 10,
            AllowedExtensions = new List<string> { ".pdf", ".doc", ".docx" }
        };
        _validator = new JobApplicationSubmitValidator(Options.Create(settings));
    }

    private static IFormFile CreateFormFile(string fileName = "resume.pdf", int sizeBytes = 100)
    {
        var bytes = new byte[sizeBytes];
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "resume", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };
    }

    private static JobApplicationSubmitCommand CreateCommand(
        long jobPostingId = 1,
        string candidateName = "Jane Doe",
        string candidateEmail = "jane@example.com",
        string? candidatePhone = "+880123456789",
        string? coverLetter = "I am interested in this role.",
        IFormFile? resume = null,
        ApplicationSourceEnum source = ApplicationSourceEnum.External)
    {
        return new JobApplicationSubmitCommand(new JobApplicationSubmitRequest
        {
            JobPostingId = jobPostingId,
            CandidateName = candidateName,
            CandidateEmail = candidateEmail,
            CandidatePhone = candidatePhone,
            CoverLetter = coverLetter,
            Resume = resume ?? CreateFormFile()
        }, source);
    }

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        // Arrange
        var command = CreateCommand();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithZeroJobPostingId_ShouldHaveError()
    {
        // Arrange
        var command = CreateCommand(jobPostingId: 0);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.JobPostingId");
    }

    [Fact]
    public void Validate_WithEmptyCandidateName_ShouldHaveError()
    {
        // Arrange
        var command = CreateCommand(candidateName: "");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.CandidateName");
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldHaveError()
    {
        // Arrange
        var command = CreateCommand(candidateEmail: "not-an-email");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.CandidateEmail");
    }

    [Fact]
    public void Validate_WithMissingResume_ShouldHaveError()
    {
        // Arrange
        var command = CreateCommand(resume: null!);
        command.Request.Resume = null;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Resume");
    }

    [Fact]
    public void Validate_WithDisallowedResumeExtension_ShouldHaveError()
    {
        // Arrange
        var command = CreateCommand(resume: CreateFormFile(fileName: "resume.exe"));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Resume");
    }

    [Fact]
    public void Validate_WithResumeExceedingMaxSize_ShouldHaveError()
    {
        // Arrange
        var oversized = CreateFormFile(sizeBytes: (10 * 1024 * 1024) + 1);
        var command = CreateCommand(resume: oversized);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Resume");
    }

    [Fact]
    public void Validate_WithInvalidPhonePattern_ShouldHaveError()
    {
        // Arrange
        var command = CreateCommand(candidatePhone: "abc-not-a-phone!!");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.CandidatePhone");
    }

    [Fact]
    public void Validate_WithOverlongCoverLetter_ShouldHaveError()
    {
        // Arrange
        var command = CreateCommand(coverLetter: new string('a', 5001));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.CoverLetter");
    }
}
