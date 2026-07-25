using FluentAssertions;
using FluentValidation;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using System.Linq.Expressions;

namespace SylviaNG.Recruitment.Tests.Services;

public class OfferLetterServiceTests
{
    private readonly Mock<IOfferLetterRepository> _offerLetterRepositoryMock;
    private readonly Mock<IDocumentTemplateRepository> _documentTemplateRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IPlaceholderSubstitutionService> _placeholderSubstitutionServiceMock;
    private readonly Mock<IOfferLetterPdfGeneratorService> _pdfGeneratorServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly OfferLetterService _service;

    public OfferLetterServiceTests()
    {
        _offerLetterRepositoryMock = new Mock<IOfferLetterRepository>();
        _documentTemplateRepositoryMock = new Mock<IDocumentTemplateRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _placeholderSubstitutionServiceMock = new Mock<IPlaceholderSubstitutionService>();
        _pdfGeneratorServiceMock = new Mock<IOfferLetterPdfGeneratorService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new OfferLetterService(
            _offerLetterRepositoryMock.Object,
            _documentTemplateRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object,
            _placeholderSubstitutionServiceMock.Object,
            _pdfGeneratorServiceMock.Object,
            _fileStorageServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private static OfferLetterGenerateRequest ValidRequest() => new()
    {
        JobApplicationId = 5,
        DocumentTemplateId = 1,
        Designation = "Software Engineer",
        OfferedSalary = 80000m,
        JoiningDate = new DateTime(2026, 8, 1),
        ReportingManager = "Jane Doe",
        OfferValidityDate = new DateTime(2026, 7, 30),
    };

    [Fact]
    public async Task GenerateAsync_TemplateNotFound_ShouldThrowNotFoundException()
    {
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((DocumentTemplate?)null);

        var act = () => _service.GenerateAsync(ValidRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GenerateAsync_WrongDocumentType_ShouldThrowValidationException()
    {
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new DocumentTemplate
        {
            DocumentTemplateId = 1,
            DocumentType = DocumentTypeEnum.AppointmentLetter,
            IsActive = true,
        });

        var act = () => _service.GenerateAsync(ValidRequest());

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GenerateAsync_InactiveTemplate_ShouldThrowValidationException()
    {
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new DocumentTemplate
        {
            DocumentTemplateId = 1,
            DocumentType = DocumentTypeEnum.OfferLetter,
            IsActive = false,
        });

        var act = () => _service.GenerateAsync(ValidRequest());

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GenerateAsync_JobApplicationNotFound_ShouldThrowNotFoundException()
    {
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new DocumentTemplate
        {
            DocumentTemplateId = 1,
            DocumentType = DocumentTypeEnum.OfferLetter,
            IsActive = true,
        });
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdWithIncludeAsync(
                It.IsAny<Expression<Func<JobApplication, bool>>>(),
                It.IsAny<Expression<Func<JobApplication, object>>[]>()))
            .ReturnsAsync((JobApplication?)null);

        var act = () => _service.GenerateAsync(ValidRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GenerateAsync_ValidRequest_ShouldRenderGeneratePersistAndReturnResponse()
    {
        var template = new DocumentTemplate
        {
            DocumentTemplateId = 1,
            DocumentType = DocumentTypeEnum.OfferLetter,
            Name = "Standard Offer Letter",
            Body = "Dear {{CandidateName}}, you are offered {{Designation}}.",
            IsActive = true,
        };
        var jobApplication = new JobApplication
        {
            JobApplicationId = 5,
            CandidateName = "John Smith",
            CandidateEmail = "john@example.com",
            JobPosting = new JobPosting { Title = "Software Engineer" },
        };

        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(template);
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdWithIncludeAsync(
                It.IsAny<Expression<Func<JobApplication, bool>>>(),
                It.IsAny<Expression<Func<JobApplication, object>>[]>()))
            .ReturnsAsync(jobApplication);
        _placeholderSubstitutionServiceMock
            .Setup(p => p.Render(template.Body, It.IsAny<Dictionary<string, string>>()))
            .Returns("Dear John Smith, you are offered Software Engineer.");
        _pdfGeneratorServiceMock
            .Setup(p => p.Generate("Standard Offer Letter", "John Smith", "Dear John Smith, you are offered Software Engineer."))
            .Returns(new byte[] { 1, 2, 3 });
        _fileStorageServiceMock
            .Setup(f => f.SaveAsync(It.IsAny<Stream>(), "offer-letter.pdf", "documents/offer-letters"))
            .ReturnsAsync(("abc123.pdf", "uploads/job-postings/documents/offer-letters/abc123.pdf"));
        _offerLetterRepositoryMock.Setup(r => r.AddAsync(It.IsAny<OfferLetter>()))
            .Callback<OfferLetter>(e => e.OfferLetterId = 42)
            .Returns(Task.CompletedTask);

        var response = await _service.GenerateAsync(ValidRequest());

        response.OfferLetterId.Should().Be(42);
        response.GeneratedPdfPath.Should().Be("uploads/job-postings/documents/offer-letters/abc123.pdf");
        response.Status.Should().Be(OfferLetterStatusEnum.Generated);
        response.CandidateName.Should().Be("John Smith");
        response.DocumentTemplateName.Should().Be("Standard Offer Letter");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ShouldThrowNotFoundException()
    {
        _offerLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(99)).ReturnsAsync((OfferLetter?)null);

        var act = () => _service.GetByIdAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedList()
    {
        _offerLetterRepositoryMock.Setup(r => r.GetAllOrderedAsync(5)).ReturnsAsync(new List<OfferLetter>
        {
            new()
            {
                OfferLetterId = 1,
                JobApplication = new JobApplication { CandidateName = "A" },
                DocumentTemplate = new DocumentTemplate { Name = "Standard" },
            },
        });

        var result = await _service.GetAllAsync(5);

        result.Should().HaveCount(1);
        result[0].CandidateName.Should().Be("A");
    }
}
