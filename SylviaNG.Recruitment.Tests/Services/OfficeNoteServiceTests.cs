using System.Linq.Expressions;
using FluentAssertions;
using FluentValidation;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class OfficeNoteServiceTests
{
    private readonly Mock<IOfficeNoteRepository> _officeNoteRepositoryMock;
    private readonly Mock<IOfferLetterRepository> _offerLetterRepositoryMock;
    private readonly Mock<IAppointmentLetterRepository> _appointmentLetterRepositoryMock;
    private readonly Mock<IJoiningBookletRepository> _joiningBookletRepositoryMock;
    private readonly Mock<IDocumentTemplateRepository> _documentTemplateRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IPlaceholderSubstitutionService> _placeholderSubstitutionServiceMock;
    private readonly Mock<IOfficeNotePdfGeneratorService> _pdfGeneratorServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly OfficeNoteService _service;

    public OfficeNoteServiceTests()
    {
        _officeNoteRepositoryMock = new Mock<IOfficeNoteRepository>();
        _offerLetterRepositoryMock = new Mock<IOfferLetterRepository>();
        _appointmentLetterRepositoryMock = new Mock<IAppointmentLetterRepository>();
        _joiningBookletRepositoryMock = new Mock<IJoiningBookletRepository>();
        _documentTemplateRepositoryMock = new Mock<IDocumentTemplateRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _placeholderSubstitutionServiceMock = new Mock<IPlaceholderSubstitutionService>();
        _pdfGeneratorServiceMock = new Mock<IOfficeNotePdfGeneratorService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Default: nothing generated for any application unless a test overrides it.
        _offerLetterRepositoryMock.Setup(r => r.GetAllOrderedAsync(It.IsAny<long?>())).ReturnsAsync(new List<OfferLetter>());
        _appointmentLetterRepositoryMock.Setup(r => r.GetAllOrderedAsync(It.IsAny<long?>())).ReturnsAsync(new List<AppointmentLetter>());
        _joiningBookletRepositoryMock.Setup(r => r.GetAllOrderedAsync(It.IsAny<long?>())).ReturnsAsync(new List<JoiningBooklet>());

        _fileStorageServiceMock
            .Setup(f => f.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(("office-note.pdf", "uploads/documents/office-notes/office-note.pdf"));
        _pdfGeneratorServiceMock
            .Setup(p => p.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()))
            .ReturnsAsync(new byte[] { 1, 2, 3 });

        _service = new OfficeNoteService(
            _officeNoteRepositoryMock.Object,
            _offerLetterRepositoryMock.Object,
            _appointmentLetterRepositoryMock.Object,
            _joiningBookletRepositoryMock.Object,
            _documentTemplateRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object,
            _placeholderSubstitutionServiceMock.Object,
            _pdfGeneratorServiceMock.Object,
            _fileStorageServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private static DocumentTemplate ActiveTemplate() => new()
    {
        DocumentTemplateId = 10,
        DocumentType = DocumentTypeEnum.OfficeNote,
        Name = "Standard Office Note",
        Body = "Dear {{CandidateName}}, enclosures: {{EnclosureList}}. Remarks: {{Remarks}}",
        IsActive = true,
    };

    private static JobApplication Application() => new()
    {
        JobApplicationId = 5,
        CandidateName = "John Smith",
        CandidateEmail = "john@example.com",
    };

    [Fact]
    public async Task GetEnclosuresAsync_ShouldReflectPresenceAndAbsence()
    {
        _offerLetterRepositoryMock.Setup(r => r.GetAllOrderedAsync(5))
            .ReturnsAsync(new List<OfferLetter> { new() { OfferLetterId = 1, GeneratedAt = DateTime.UtcNow } });

        var response = await _service.GetEnclosuresAsync(5);

        response.Enclosures.Should().HaveCount(3);
        response.Enclosures.Single(e => e.DocumentType == DocumentTypeEnum.OfferLetter).Exists.Should().BeTrue();
        response.Enclosures.Single(e => e.DocumentType == DocumentTypeEnum.AppointmentLetter).Exists.Should().BeFalse();
        response.Enclosures.Single(e => e.DocumentType == DocumentTypeEnum.JoiningBooklet).Exists.Should().BeFalse();
    }

    [Fact]
    public async Task GenerateAsync_TemplateNotFound_ShouldThrowNotFoundException()
    {
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((DocumentTemplate?)null);

        var act = () => _service.GenerateAsync(new OfficeNoteGenerateRequest { JobApplicationId = 5, DocumentTemplateId = 10 });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GenerateAsync_WrongTemplateType_ShouldThrowValidationException()
    {
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new DocumentTemplate
        {
            DocumentTemplateId = 10,
            DocumentType = DocumentTypeEnum.OfferLetter,
            IsActive = true,
        });

        var act = () => _service.GenerateAsync(new OfficeNoteGenerateRequest { JobApplicationId = 5, DocumentTemplateId = 10 });

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GenerateAsync_InactiveTemplate_ShouldThrowValidationException()
    {
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new DocumentTemplate
        {
            DocumentTemplateId = 10,
            DocumentType = DocumentTypeEnum.OfficeNote,
            IsActive = false,
        });

        var act = () => _service.GenerateAsync(new OfficeNoteGenerateRequest { JobApplicationId = 5, DocumentTemplateId = 10 });

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GenerateAsync_NoEnclosuresExist_ShouldThrowValidationException()
    {
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(ActiveTemplate());
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByIdWithIncludeAsync(It.IsAny<Expression<Func<JobApplication, bool>>>(), It.IsAny<Expression<Func<JobApplication, object>>[]>()))
            .ReturnsAsync(Application());

        var act = () => _service.GenerateAsync(new OfficeNoteGenerateRequest { JobApplicationId = 5, DocumentTemplateId = 10 });

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GenerateAsync_ValidRequest_ShouldPersistEntityWithEnclosuresSummaryAndReturnResponse()
    {
        var template = ActiveTemplate();
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(template);
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByIdWithIncludeAsync(It.IsAny<Expression<Func<JobApplication, bool>>>(), It.IsAny<Expression<Func<JobApplication, object>>[]>()))
            .ReturnsAsync(Application());
        _offerLetterRepositoryMock.Setup(r => r.GetAllOrderedAsync(5))
            .ReturnsAsync(new List<OfferLetter> { new() { OfferLetterId = 1, GeneratedAt = DateTime.UtcNow } });
        _joiningBookletRepositoryMock.Setup(r => r.GetAllOrderedAsync(5))
            .ReturnsAsync(new List<JoiningBooklet> { new() { JoiningBookletId = 2, GeneratedAt = DateTime.UtcNow } });
        _placeholderSubstitutionServiceMock
            .Setup(p => p.Render(template.Body, It.IsAny<Dictionary<string, string>>()))
            .Returns("Dear John Smith, enclosures: Offer Letter, Joining Booklet. Remarks: Please expedite.");
        _officeNoteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<OfficeNote>()))
            .Callback<OfficeNote>(e => e.OfficeNoteId = 42)
            .Returns(Task.CompletedTask);

        var response = await _service.GenerateAsync(new OfficeNoteGenerateRequest
        {
            JobApplicationId = 5,
            DocumentTemplateId = 10,
            Remarks = "Please expedite.",
        });

        response.OfficeNoteId.Should().Be(42);
        response.EnclosuresSummary.Should().Be("Offer Letter, Joining Booklet");
        response.GeneratedPdfPath.Should().Be("uploads/documents/office-notes/office-note.pdf");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _officeNoteRepositoryMock.Verify(r => r.AddAsync(It.Is<OfficeNote>(
            e => e.EnclosuresSummary == "Offer Letter, Joining Booklet" && e.Remarks == "Please expedite.")), Times.Once);
    }
}
