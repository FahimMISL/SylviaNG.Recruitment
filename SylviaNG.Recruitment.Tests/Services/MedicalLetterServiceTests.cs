using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Options;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class MedicalLetterServiceTests
{
    private readonly Mock<IMedicalLetterRepository> _medicalLetterRepositoryMock;
    private readonly Mock<IOfferLetterRepository> _offerLetterRepositoryMock;
    private readonly Mock<IDocumentTemplateRepository> _documentTemplateRepositoryMock;
    private readonly Mock<IMedicalLetterPdfGeneratorService> _pdfGeneratorServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<INotificationDispatchService> _notificationDispatchServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MedicalLetterService _service;

    public MedicalLetterServiceTests()
    {
        _medicalLetterRepositoryMock = new Mock<IMedicalLetterRepository>();
        _offerLetterRepositoryMock = new Mock<IOfferLetterRepository>();
        _documentTemplateRepositoryMock = new Mock<IDocumentTemplateRepository>();
        _pdfGeneratorServiceMock = new Mock<IMedicalLetterPdfGeneratorService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _notificationDispatchServiceMock = new Mock<INotificationDispatchService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new MedicalLetterService(
            _medicalLetterRepositoryMock.Object,
            _offerLetterRepositoryMock.Object,
            _documentTemplateRepositoryMock.Object,
            _pdfGeneratorServiceMock.Object,
            _fileStorageServiceMock.Object,
            _notificationDispatchServiceMock.Object,
            Options.Create(new PortalSettings()),
            _unitOfWorkMock.Object);

        _fileStorageServiceMock
            .Setup(f => f.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(("abc.pdf", "uploads/documents/medical-letters/abc.pdf"));
        _pdfGeneratorServiceMock
            .Setup(p => p.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new byte[] { 1, 2, 3 });
    }

    private static OfferLetter AcceptedOfferLetter() => new()
    {
        OfferLetterId = 1,
        JobApplicationId = 5,
        Status = OfferLetterStatusEnum.Accepted,
        JobApplication = new JobApplication { JobApplicationId = 5, CandidateName = "John Smith", CandidateEmail = "john@example.com" },
    };

    private static MedicalLetterGenerateRequest ValidRequest() => new()
    {
        OfferLetterId = 1,
        DocumentTemplateId = 10,
        MedicalTestCenter = "City Diagnostics",
        RequiredTests = "Blood Test, X-Ray",
        FinalBody = "Dear John Smith, please visit City Diagnostics.",
    };

    [Fact]
    public async Task GenerateAsync_OfferLetterNotFound_ShouldThrowNotFoundException()
    {
        _offerLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync((OfferLetter?)null);

        var act = () => _service.GenerateAsync(ValidRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GenerateAsync_OfferNotAccepted_ShouldThrowValidationException()
    {
        var offerLetter = AcceptedOfferLetter();
        offerLetter.Status = OfferLetterStatusEnum.Generated;
        _offerLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(offerLetter);

        var act = () => _service.GenerateAsync(ValidRequest());

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GenerateAsync_WrongDocumentType_ShouldThrowValidationException()
    {
        _offerLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(AcceptedOfferLetter());
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new DocumentTemplate
        {
            DocumentTemplateId = 10,
            DocumentType = DocumentTypeEnum.TargetLetter,
            IsActive = true,
        });

        var act = () => _service.GenerateAsync(ValidRequest());

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GenerateAsync_InactiveTemplate_ShouldThrowValidationException()
    {
        _offerLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(AcceptedOfferLetter());
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new DocumentTemplate
        {
            DocumentTemplateId = 10,
            DocumentType = DocumentTypeEnum.MedicalReferral,
            IsActive = false,
        });

        var act = () => _service.GenerateAsync(ValidRequest());

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GenerateAsync_ValidRequest_ShouldPersistAndReturnResponse()
    {
        _offerLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(AcceptedOfferLetter());
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new DocumentTemplate
        {
            DocumentTemplateId = 10,
            DocumentType = DocumentTypeEnum.MedicalReferral,
            Name = "Standard Medical Letter",
            IsActive = true,
        });
        _medicalLetterRepositoryMock.Setup(r => r.AddAsync(It.IsAny<MedicalLetter>()))
            .Callback<MedicalLetter>(e => e.MedicalLetterId = 42)
            .Returns(Task.CompletedTask);

        var response = await _service.GenerateAsync(ValidRequest());

        response.MedicalLetterId.Should().Be(42);
        response.GeneratedPdfPath.Should().Be("uploads/documents/medical-letters/abc.pdf");
        response.MedicalTestCenter.Should().Be("City Diagnostics");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _notificationDispatchServiceMock.Verify(n => n.DispatchAsync(
            RecruitmentEventEnum.MedicalLetterAvailable,
            It.IsAny<IDictionary<string, string>>(),
            It.IsAny<NotificationDispatchTargets>(),
            true,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ShouldThrowNotFoundException()
    {
        _medicalLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(99)).ReturnsAsync((MedicalLetter?)null);

        var act = () => _service.GetByIdAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
