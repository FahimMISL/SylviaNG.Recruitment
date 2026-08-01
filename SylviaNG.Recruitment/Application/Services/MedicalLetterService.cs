using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class MedicalLetterService : IMedicalLetterService
    {
        private readonly IMedicalLetterRepository _medicalLetterRepository;
        private readonly IOfferLetterRepository _offerLetterRepository;
        private readonly IDocumentTemplateRepository _documentTemplateRepository;
        private readonly IMedicalLetterPdfGeneratorService _medicalLetterPdfGeneratorService;
        private readonly IFileStorageService _fileStorageService;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly PortalSettings _portalSettings;
        private readonly IUnitOfWork _unitOfWork;

        private const string PdfStorageSubFolder = "documents/medical-letters";

        public MedicalLetterService(
            IMedicalLetterRepository medicalLetterRepository,
            IOfferLetterRepository offerLetterRepository,
            IDocumentTemplateRepository documentTemplateRepository,
            IMedicalLetterPdfGeneratorService medicalLetterPdfGeneratorService,
            IFileStorageService fileStorageService,
            INotificationDispatchService notificationDispatchService,
            IApplicationSettingService applicationSettingService,
            IOptions<PortalSettings> portalSettings,
            IUnitOfWork unitOfWork)
        {
            _medicalLetterRepository = medicalLetterRepository;
            _offerLetterRepository = offerLetterRepository;
            _documentTemplateRepository = documentTemplateRepository;
            _medicalLetterPdfGeneratorService = medicalLetterPdfGeneratorService;
            _fileStorageService = fileStorageService;
            _notificationDispatchService = notificationDispatchService;
            _applicationSettingService = applicationSettingService;
            _portalSettings = portalSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<MedicalLetterResponse> GenerateAsync(MedicalLetterGenerateRequest request)
        {
            var offerLetter = await _offerLetterRepository.GetByIdWithDetailsAsync(request.OfferLetterId)
                ?? throw new NotFoundException("OfferLetter", request.OfferLetterId);

            // AC3: medical letters are only generated once the offer has been accepted, same
            // gate as AppointmentLetter.
            if (offerLetter.Status != OfferLetterStatusEnum.Accepted)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.OfferLetterId),
                        "A medical letter can only be generated for an Accepted offer letter.")
                });

            var template = await _documentTemplateRepository.GetByIdAsync(request.DocumentTemplateId)
                ?? throw new NotFoundException("DocumentTemplate", request.DocumentTemplateId);

            if (template.DocumentType != DocumentTypeEnum.MedicalReferral)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DocumentTemplateId),
                        "The selected template is not a MedicalReferral-type DocumentTemplate.")
                });

            if (!template.IsActive)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DocumentTemplateId), "The selected template is inactive.")
                });

            var pdfBytes = await _medicalLetterPdfGeneratorService.Generate(
                template.Name, offerLetter.JobApplication.CandidateName, request.FinalBody, offerLetter.OfferLetterId);

            using var pdfStream = new MemoryStream(pdfBytes);
            var (_, filePath) = await _fileStorageService.SaveAsync(pdfStream, "medical-letter.pdf", PdfStorageSubFolder);

            var entity = new MedicalLetter
            {
                JobApplicationId = offerLetter.JobApplicationId,
                OfferLetterId = offerLetter.OfferLetterId,
                DocumentTemplateId = request.DocumentTemplateId,
                MedicalTestCenter = request.MedicalTestCenter,
                RequiredTests = request.RequiredTests,
                FinalBody = request.FinalBody,
                GeneratedPdfPath = filePath,
                GeneratedAt = DateTime.UtcNow,
            };

            await _medicalLetterRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            entity.JobApplication = offerLetter.JobApplication;
            entity.OfferLetter = offerLetter;
            entity.DocumentTemplate = template;

            // AC5: candidate notified. Never throws - a missing EventTemplateMapping just logs a
            // Skipped NotificationLog row.
            var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = offerLetter.JobApplication.CandidateName,
                ["CandidateReference"] = $"JA-{offerLetter.JobApplicationId}",
                ["MedicalTestCenter"] = request.MedicalTestCenter,
                ["RequiredTests"] = request.RequiredTests,
                ["PortalLink"] = $"{_portalSettings.FrontendBaseUrl}/candidate-profile/offer-letters",
            };

            await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.MedicalLetterAvailable,
                placeholders,
                new NotificationDispatchTargets(offerLetter.JobApplication.CandidateEmail, await _applicationSettingService.GetHrNotificationEmailAsync(), offerLetter.JobApplicationId),
                persistImmediately: true);

            return entity.ToResponse();
        }

        public async Task<List<MedicalLetterResponse>> GetAllAsync(long? jobApplicationId)
        {
            var entities = await _medicalLetterRepository.GetAllOrderedAsync(jobApplicationId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<MedicalLetterResponse> GetByIdAsync(long medicalLetterId)
        {
            var entity = await _medicalLetterRepository.GetByIdWithDetailsAsync(medicalLetterId)
                ?? throw new NotFoundException("MedicalLetter", medicalLetterId);

            return entity.ToResponse();
        }
    }
}
