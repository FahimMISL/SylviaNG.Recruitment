using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class AppointmentLetterService : IAppointmentLetterService
    {
        private readonly IAppointmentLetterRepository _appointmentLetterRepository;
        private readonly IOfferLetterRepository _offerLetterRepository;
        private readonly IDocumentTemplateRepository _documentTemplateRepository;
        private readonly IAppointmentLetterPdfGeneratorService _appointmentLetterPdfGeneratorService;
        private readonly IFileStorageService _fileStorageService;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly PortalSettings _portalSettings;
        private readonly IUnitOfWork _unitOfWork;

        private const string PdfStorageSubFolder = "documents/appointment-letters";

        public AppointmentLetterService(
            IAppointmentLetterRepository appointmentLetterRepository,
            IOfferLetterRepository offerLetterRepository,
            IDocumentTemplateRepository documentTemplateRepository,
            IAppointmentLetterPdfGeneratorService appointmentLetterPdfGeneratorService,
            IFileStorageService fileStorageService,
            INotificationDispatchService notificationDispatchService,
            IApplicationSettingService applicationSettingService,
            IOptions<PortalSettings> portalSettings,
            IUnitOfWork unitOfWork)
        {
            _appointmentLetterRepository = appointmentLetterRepository;
            _offerLetterRepository = offerLetterRepository;
            _documentTemplateRepository = documentTemplateRepository;
            _appointmentLetterPdfGeneratorService = appointmentLetterPdfGeneratorService;
            _fileStorageService = fileStorageService;
            _notificationDispatchService = notificationDispatchService;
            _applicationSettingService = applicationSettingService;
            _portalSettings = portalSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentLetterResponse> GenerateAsync(AppointmentLetterGenerateRequest request)
        {
            var offerLetter = await _offerLetterRepository.GetByIdWithDetailsAsync(request.OfferLetterId)
                ?? throw new NotFoundException("OfferLetter", request.OfferLetterId);

            // AC1: Generate Appointment Letter is only available after the offer is accepted.
            if (offerLetter.Status != OfferLetterStatusEnum.Accepted)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.OfferLetterId),
                        "An appointment letter can only be generated for an Accepted offer letter.")
                });

            var template = await _documentTemplateRepository.GetByIdAsync(request.DocumentTemplateId)
                ?? throw new NotFoundException("DocumentTemplate", request.DocumentTemplateId);

            if (template.DocumentType != DocumentTypeEnum.AppointmentLetter)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DocumentTemplateId),
                        "The selected template is not an AppointmentLetter-type DocumentTemplate.")
                });

            if (!template.IsActive)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DocumentTemplateId),
                        "The selected template is inactive.")
                });

            var pdfBytes = await _appointmentLetterPdfGeneratorService.Generate(
                template.Name, offerLetter.JobApplication.CandidateName, request.FinalBody, offerLetter.OfferLetterId);

            using var pdfStream = new MemoryStream(pdfBytes);
            var (_, filePath) = await _fileStorageService.SaveAsync(pdfStream, "appointment-letter.pdf", PdfStorageSubFolder);

            var entity = new AppointmentLetter
            {
                JobApplicationId = offerLetter.JobApplicationId,
                OfferLetterId = offerLetter.OfferLetterId,
                DocumentTemplateId = request.DocumentTemplateId,
                FinalBody = request.FinalBody,
                GeneratedPdfPath = filePath,
                GeneratedAt = DateTime.UtcNow,
            };

            await _appointmentLetterRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            entity.JobApplication = offerLetter.JobApplication;
            entity.OfferLetter = offerLetter;
            entity.DocumentTemplate = template;

            // AC4: delivered to the candidate via email; HR gets a copy too (same dual-target
            // pattern as JobApplicationService.BuildDispatchTargetsAsync). Never throws.
            var hrEmail = await _applicationSettingService.GetHrNotificationEmailAsync();
            var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = offerLetter.JobApplication.CandidateName,
                ["Designation"] = offerLetter.Designation,
                ["JoiningDate"] = offerLetter.JoiningDate.ToString("dd MMM yyyy"),
                ["PortalLink"] = $"{_portalSettings.FrontendBaseUrl}/candidate-profile/offer-letters",
            };

            await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.AppointmentLetterGenerated,
                placeholders,
                new NotificationDispatchTargets(offerLetter.JobApplication.CandidateEmail, hrEmail, offerLetter.JobApplicationId),
                persistImmediately: true);

            return entity.ToResponse();
        }

        public async Task<List<AppointmentLetterResponse>> GetAllAsync(long? jobApplicationId)
        {
            var entities = await _appointmentLetterRepository.GetAllOrderedAsync(jobApplicationId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<AppointmentLetterResponse> GetByIdAsync(long appointmentLetterId)
        {
            var entity = await _appointmentLetterRepository.GetByIdWithDetailsAsync(appointmentLetterId)
                ?? throw new NotFoundException("AppointmentLetter", appointmentLetterId);

            return entity.ToResponse();
        }
    }
}
