using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Options;
using Moq;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.TargetLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class TargetLetterServiceTests
{
    private readonly Mock<ITargetLetterRepository> _targetLetterRepositoryMock;
    private readonly Mock<IOfferLetterRepository> _offerLetterRepositoryMock;
    private readonly Mock<IDocumentTemplateRepository> _documentTemplateRepositoryMock;
    private readonly Mock<ITargetLetterPdfGeneratorService> _pdfGeneratorServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<INotificationDispatchService> _notificationDispatchServiceMock;
    private readonly Mock<IApplicationSettingService> _applicationSettingServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TargetLetterService _service;

    public TargetLetterServiceTests()
    {
        _targetLetterRepositoryMock = new Mock<ITargetLetterRepository>();
        _offerLetterRepositoryMock = new Mock<IOfferLetterRepository>();
        _documentTemplateRepositoryMock = new Mock<IDocumentTemplateRepository>();
        _pdfGeneratorServiceMock = new Mock<ITargetLetterPdfGeneratorService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _notificationDispatchServiceMock = new Mock<INotificationDispatchService>();
        _applicationSettingServiceMock = new Mock<IApplicationSettingService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _applicationSettingServiceMock.Setup(s => s.GetHrNotificationEmailAsync()).ReturnsAsync((string?)null);

        _service = new TargetLetterService(
            _targetLetterRepositoryMock.Object,
            _offerLetterRepositoryMock.Object,
            _documentTemplateRepositoryMock.Object,
            _pdfGeneratorServiceMock.Object,
            _fileStorageServiceMock.Object,
            _notificationDispatchServiceMock.Object,
            _applicationSettingServiceMock.Object,
            Options.Create(new PortalSettings()),
            _unitOfWorkMock.Object);

        _fileStorageServiceMock
            .Setup(f => f.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(("abc.pdf", "uploads/documents/target-letters/abc.pdf"));
        _pdfGeneratorServiceMock
            .Setup(p => p.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()))
            .ReturnsAsync(new byte[] { 1, 2, 3 });
    }

    private static OfferLetter AcceptedOfferLetter() => new()
    {
        OfferLetterId = 1,
        JobApplicationId = 5,
        Designation = "Software Engineer",
        Status = OfferLetterStatusEnum.Accepted,
        JobApplication = new JobApplication { JobApplicationId = 5, CandidateName = "John Smith", CandidateEmail = "john@example.com" },
    };

    private static TargetLetterGenerateRequest ValidRequest() => new()
    {
        OfferLetterId = 1,
        DocumentTemplateId = 10,
        Kpis = "Close 5 deals/quarter",
        Objectives = "Ramp up within 90 days",
        FinalBody = "Dear John Smith, your KPIs are...",
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
            DocumentType = DocumentTypeEnum.MedicalReferral,
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
            DocumentType = DocumentTypeEnum.TargetLetter,
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
            DocumentType = DocumentTypeEnum.TargetLetter,
            Name = "Standard Target Letter",
            IsActive = true,
        });
        _targetLetterRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TargetLetter>()))
            .Callback<TargetLetter>(e => e.TargetLetterId = 42)
            .Returns(Task.CompletedTask);

        var response = await _service.GenerateAsync(ValidRequest());

        response.TargetLetterId.Should().Be(42);
        response.GeneratedPdfPath.Should().Be("recruitment/files/download?key=uploads%2Fdocuments%2Ftarget-letters%2Fabc.pdf");
        response.Kpis.Should().Be("Close 5 deals/quarter");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _notificationDispatchServiceMock.Verify(n => n.DispatchAsync(
            RecruitmentEventEnum.TargetLetterAvailable,
            It.IsAny<IDictionary<string, string>>(),
            It.IsAny<NotificationDispatchTargets>(),
            true,
            It.IsAny<IReadOnlyList<EmailAttachment>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ShouldThrowNotFoundException()
    {
        _targetLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(99)).ReturnsAsync((TargetLetter?)null);

        var act = () => _service.GetByIdAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
