using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.TargetLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class TargetLetterService : ITargetLetterService
    {
        private readonly ITargetLetterRepository _targetLetterRepository;
        private readonly IOfferLetterRepository _offerLetterRepository;
        private readonly IDocumentTemplateRepository _documentTemplateRepository;
        private readonly ITargetLetterPdfGeneratorService _targetLetterPdfGeneratorService;
        private readonly IFileStorageService _fileStorageService;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly PortalSettings _portalSettings;
        private readonly IUnitOfWork _unitOfWork;

        private const string PdfStorageSubFolder = "documents/target-letters";

        public TargetLetterService(
            ITargetLetterRepository targetLetterRepository,
            IOfferLetterRepository offerLetterRepository,
            IDocumentTemplateRepository documentTemplateRepository,
            ITargetLetterPdfGeneratorService targetLetterPdfGeneratorService,
            IFileStorageService fileStorageService,
            INotificationDispatchService notificationDispatchService,
            IOptions<PortalSettings> portalSettings,
            IUnitOfWork unitOfWork)
        {
            _targetLetterRepository = targetLetterRepository;
            _offerLetterRepository = offerLetterRepository;
            _documentTemplateRepository = documentTemplateRepository;
            _targetLetterPdfGeneratorService = targetLetterPdfGeneratorService;
            _fileStorageService = fileStorageService;
            _notificationDispatchService = notificationDispatchService;
            _portalSettings = portalSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<TargetLetterResponse> GenerateAsync(TargetLetterGenerateRequest request)
        {
            var offerLetter = await _offerLetterRepository.GetByIdWithDetailsAsync(request.OfferLetterId)
                ?? throw new NotFoundException("OfferLetter", request.OfferLetterId);

            // AC3: target letters are only generated once the offer has been accepted, same gate
            // as AppointmentLetter.
            if (offerLetter.Status != OfferLetterStatusEnum.Accepted)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.OfferLetterId),
                        "A target letter can only be generated for an Accepted offer letter.")
                });

            var template = await _documentTemplateRepository.GetByIdAsync(request.DocumentTemplateId)
                ?? throw new NotFoundException("DocumentTemplate", request.DocumentTemplateId);

            if (template.DocumentType != DocumentTypeEnum.TargetLetter)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DocumentTemplateId),
                        "The selected template is not a TargetLetter-type DocumentTemplate.")
                });

            if (!template.IsActive)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DocumentTemplateId), "The selected template is inactive.")
                });

            var pdfBytes = await _targetLetterPdfGeneratorService.Generate(
                template.Name, offerLetter.JobApplication.CandidateName, request.FinalBody, offerLetter.OfferLetterId);

            using var pdfStream = new MemoryStream(pdfBytes);
            var (_, filePath) = await _fileStorageService.SaveAsync(pdfStream, "target-letter.pdf", PdfStorageSubFolder);

            var entity = new TargetLetter
            {
                JobApplicationId = offerLetter.JobApplicationId,
                OfferLetterId = offerLetter.OfferLetterId,
                DocumentTemplateId = request.DocumentTemplateId,
                Kpis = request.Kpis,
                Objectives = request.Objectives,
                FinalBody = request.FinalBody,
                GeneratedPdfPath = filePath,
                GeneratedAt = DateTime.UtcNow,
            };

            await _targetLetterRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            entity.JobApplication = offerLetter.JobApplication;
            entity.OfferLetter = offerLetter;
            entity.DocumentTemplate = template;

            // AC5: candidate notified. Never throws - a missing EventTemplateMapping just logs a
            // Skipped NotificationLog row.
            var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = offerLetter.JobApplication.CandidateName,
                ["Designation"] = offerLetter.Designation,
                ["Kpis"] = request.Kpis,
                ["Objectives"] = request.Objectives,
                ["PortalLink"] = $"{_portalSettings.FrontendBaseUrl}/candidate-profile/offer-letters",
            };

            await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.TargetLetterAvailable,
                placeholders,
                new NotificationDispatchTargets(offerLetter.JobApplication.CandidateEmail, null, offerLetter.JobApplicationId),
                persistImmediately: true);

            return entity.ToResponse();
        }

        public async Task<List<TargetLetterResponse>> GetAllAsync(long? jobApplicationId)
        {
            var entities = await _targetLetterRepository.GetAllOrderedAsync(jobApplicationId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<TargetLetterResponse> GetByIdAsync(long targetLetterId)
        {
            var entity = await _targetLetterRepository.GetByIdWithDetailsAsync(targetLetterId)
                ?? throw new NotFoundException("TargetLetter", targetLetterId);

            return entity.ToResponse();
        }
    }
}
