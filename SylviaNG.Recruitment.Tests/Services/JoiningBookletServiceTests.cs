using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Options;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class JoiningBookletServiceTests
{
    private readonly Mock<IJoiningBookletRepository> _joiningBookletRepositoryMock;
    private readonly Mock<IOfferLetterRepository> _offerLetterRepositoryMock;
    private readonly Mock<IDocumentTemplateRepository> _documentTemplateRepositoryMock;
    private readonly Mock<IPlaceholderSubstitutionService> _placeholderSubstitutionServiceMock;
    private readonly Mock<IJoiningBookletPdfGeneratorService> _pdfGeneratorServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<INotificationDispatchService> _notificationDispatchServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly JoiningBookletService _service;

    public JoiningBookletServiceTests()
    {
        _joiningBookletRepositoryMock = new Mock<IJoiningBookletRepository>();
        _offerLetterRepositoryMock = new Mock<IOfferLetterRepository>();
        _documentTemplateRepositoryMock = new Mock<IDocumentTemplateRepository>();
        _placeholderSubstitutionServiceMock = new Mock<IPlaceholderSubstitutionService>();
        _pdfGeneratorServiceMock = new Mock<IJoiningBookletPdfGeneratorService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _notificationDispatchServiceMock = new Mock<INotificationDispatchService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new JoiningBookletService(
            _joiningBookletRepositoryMock.Object,
            _offerLetterRepositoryMock.Object,
            _documentTemplateRepositoryMock.Object,
            _placeholderSubstitutionServiceMock.Object,
            _pdfGeneratorServiceMock.Object,
            _fileStorageServiceMock.Object,
            _notificationDispatchServiceMock.Object,
            Options.Create(new PortalSettings()),
            _unitOfWorkMock.Object);

        _fileStorageServiceMock
            .Setup(f => f.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(("abc.pdf", "uploads/documents/joining-booklets/abc.pdf"));
        _pdfGeneratorServiceMock
            .Setup(p => p.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new byte[] { 1, 2, 3 });
    }

    private static OfferLetter AcceptedOfferLetter(long offerLetterId = 1) => new()
    {
        OfferLetterId = offerLetterId,
        JobApplicationId = 5,
        Designation = "Software Engineer",
        Status = OfferLetterStatusEnum.Accepted,
        JobApplication = new JobApplication { JobApplicationId = 5, CandidateName = "John Smith", CandidateEmail = "john@example.com" },
    };

    private static DocumentTemplate ActiveTemplate() => new()
    {
        DocumentTemplateId = 10,
        DocumentType = DocumentTypeEnum.JoiningBooklet,
        Name = "Standard Joining Booklet",
        Body = "Dear {{CandidateName}}, batch {{BatchLabel}}.",
        IsActive = true,
    };

    [Fact]
    public async Task GenerateAsync_OfferLetterNotFound_ShouldThrowNotFoundException()
    {
        _offerLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync((OfferLetter?)null);

        var act = () => _service.GenerateAsync(new JoiningBookletGenerateRequest { OfferLetterId = 1, DocumentTemplateId = 10, BatchLabel = "B1", JoiningDate = DateTime.UtcNow });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GenerateAsync_OfferNotAccepted_ShouldThrowValidationException()
    {
        var offerLetter = AcceptedOfferLetter();
        offerLetter.Status = OfferLetterStatusEnum.Generated;
        _offerLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(offerLetter);

        var act = () => _service.GenerateAsync(new JoiningBookletGenerateRequest { OfferLetterId = 1, DocumentTemplateId = 10, BatchLabel = "B1", JoiningDate = DateTime.UtcNow });

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GenerateAsync_WrongTemplateType_ShouldThrowValidationException()
    {
        _offerLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(AcceptedOfferLetter());
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new DocumentTemplate
        {
            DocumentTemplateId = 10,
            DocumentType = DocumentTypeEnum.OfferLetter,
            IsActive = true,
        });

        var act = () => _service.GenerateAsync(new JoiningBookletGenerateRequest { OfferLetterId = 1, DocumentTemplateId = 10, BatchLabel = "B1", JoiningDate = DateTime.UtcNow });

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GenerateAsync_InactiveTemplate_ShouldThrowValidationException()
    {
        _offerLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(AcceptedOfferLetter());
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new DocumentTemplate
        {
            DocumentTemplateId = 10,
            DocumentType = DocumentTypeEnum.JoiningBooklet,
            IsActive = false,
        });

        var act = () => _service.GenerateAsync(new JoiningBookletGenerateRequest { OfferLetterId = 1, DocumentTemplateId = 10, BatchLabel = "B1", JoiningDate = DateTime.UtcNow });

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GenerateAsync_ValidRequest_ShouldRenderGeneratePersistAndReturnResponse()
    {
        var offerLetter = AcceptedOfferLetter();
        var template = ActiveTemplate();
        _offerLetterRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(offerLetter);
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(template);
        _placeholderSubstitutionServiceMock
            .Setup(p => p.Render(template.Body, It.IsAny<Dictionary<string, string>>()))
            .Returns("Dear John Smith, batch B1.");
        _joiningBookletRepositoryMock.Setup(r => r.AddAsync(It.IsAny<JoiningBooklet>()))
            .Callback<JoiningBooklet>(e => e.JoiningBookletId = 42)
            .Returns(Task.CompletedTask);

        var response = await _service.GenerateAsync(new JoiningBookletGenerateRequest
        {
            OfferLetterId = 1,
            DocumentTemplateId = 10,
            BatchLabel = "B1",
            JoiningDate = new DateTime(2026, 8, 1),
        });

        response.JoiningBookletId.Should().Be(42);
        response.RenderedBody.Should().Be("Dear John Smith, batch B1.");
        response.GeneratedPdfPath.Should().Be("uploads/documents/joining-booklets/abc.pdf");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _notificationDispatchServiceMock.Verify(n => n.DispatchAsync(
            RecruitmentEventEnum.JoiningBookletAvailable,
            It.IsAny<IDictionary<string, string>>(),
            It.IsAny<NotificationDispatchTargets>(),
            true,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BulkGenerateAsync_EmptyOfferLetterIds_ShouldThrowValidationException()
    {
        var act = () => _service.BulkGenerateAsync(new JoiningBookletBulkGenerateRequest
        {
            OfferLetterIds = new List<long>(),
            DocumentTemplateId = 10,
            BatchLabel = "B1",
            JoiningDate = DateTime.UtcNow,
        });

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task BulkGenerateAsync_MixedSuccessAndFailure_ShouldReportBothWithoutThrowing()
    {
        var template = ActiveTemplate();
        var validOffer = AcceptedOfferLetter(1);
        _documentTemplateRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(template);
        _offerLetterRepositoryMock.Setup(r => r.GetByIdsWithDetailsAsync(It.IsAny<List<long>>()))
            .ReturnsAsync(new List<OfferLetter> { validOffer });
        _placeholderSubstitutionServiceMock
            .Setup(p => p.Render(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
            .Returns("rendered");
        _joiningBookletRepositoryMock.Setup(r => r.AddAsync(It.IsAny<JoiningBooklet>()))
            .Callback<JoiningBooklet>(e => e.JoiningBookletId = 42)
            .Returns(Task.CompletedTask);

        var response = await _service.BulkGenerateAsync(new JoiningBookletBulkGenerateRequest
        {
            OfferLetterIds = new List<long> { 1, 999 },
            DocumentTemplateId = 10,
            BatchLabel = "B1",
            JoiningDate = DateTime.UtcNow,
        });

        response.TotalRequested.Should().Be(2);
        response.SuccessCount.Should().Be(1);
        response.FailureCount.Should().Be(1);
        response.Results.Should().ContainSingle(r => r.OfferLetterId == 1 && r.Success);
        response.Results.Should().ContainSingle(r => r.OfferLetterId == 999 && !r.Success);
    }

    [Fact]
    public async Task BulkDownloadAsync_EmptyIds_ShouldThrowValidationException()
    {
        var act = () => _service.BulkDownloadAsync(new JoiningBookletBulkDownloadRequest { JoiningBookletIds = new List<long>() });

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task BulkDownloadAsync_ShouldRegenerateFromRenderedBodyWithoutTouchingFileStorage()
    {
        var booklet = new JoiningBooklet
        {
            JoiningBookletId = 1,
            RenderedBody = "Dear John Smith.",
            JobApplication = new JobApplication { CandidateName = "John Smith" },
            DocumentTemplate = new DocumentTemplate { Name = "Standard Joining Booklet" },
        };
        _joiningBookletRepositoryMock.Setup(r => r.GetByIdsWithDetailsAsync(It.IsAny<List<long>>()))
            .ReturnsAsync(new List<JoiningBooklet> { booklet });

        var response = await _service.BulkDownloadAsync(new JoiningBookletBulkDownloadRequest { JoiningBookletIds = new List<long> { 1 } });

        response.ContentType.Should().Be("application/zip");
        response.Content.Should().NotBeEmpty();
        _pdfGeneratorServiceMock.Verify(p => p.Generate("Standard Joining Booklet", "John Smith", "Dear John Smith."), Times.Once);
        _fileStorageServiceMock.Verify(f => f.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
