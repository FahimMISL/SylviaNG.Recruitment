using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class OfferLetterService : IOfferLetterService
    {
        private readonly IOfferLetterRepository _offerLetterRepository;
        private readonly IDocumentTemplateRepository _documentTemplateRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IPlaceholderSubstitutionService _placeholderSubstitutionService;
        private readonly IOfferLetterPdfGeneratorService _offerLetterPdfGeneratorService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly PortalSettings _portalSettings;
        private readonly IUnitOfWork _unitOfWork;

        private const string PdfStorageSubFolder = "documents/offer-letters";

        public OfferLetterService(
            IOfferLetterRepository offerLetterRepository,
            IDocumentTemplateRepository documentTemplateRepository,
            IJobApplicationRepository jobApplicationRepository,
            IPlaceholderSubstitutionService placeholderSubstitutionService,
            IOfferLetterPdfGeneratorService offerLetterPdfGeneratorService,
            IFileStorageService fileStorageService,
            ICurrentCandidateService currentCandidateService,
            INotificationDispatchService notificationDispatchService,
            IApplicationSettingService applicationSettingService,
            IOptions<PortalSettings> portalSettings,
            IUnitOfWork unitOfWork)
        {
            _offerLetterRepository = offerLetterRepository;
            _documentTemplateRepository = documentTemplateRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _placeholderSubstitutionService = placeholderSubstitutionService;
            _offerLetterPdfGeneratorService = offerLetterPdfGeneratorService;
            _fileStorageService = fileStorageService;
            _currentCandidateService = currentCandidateService;
            _notificationDispatchService = notificationDispatchService;
            _applicationSettingService = applicationSettingService;
            _portalSettings = portalSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<OfferLetterResponse> GenerateAsync(OfferLetterGenerateRequest request)
        {
            var template = await _documentTemplateRepository.GetByIdAsync(request.DocumentTemplateId)
                ?? throw new NotFoundException("DocumentTemplate", request.DocumentTemplateId);

            if (template.DocumentType != DocumentTypeEnum.OfferLetter)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DocumentTemplateId),
                        "The selected template is not an OfferLetter-type DocumentTemplate.")
                });

            if (!template.IsActive)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DocumentTemplateId),
                        "The selected template is inactive.")
                });

            var jobApplication = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == request.JobApplicationId,
                a => a.CandidateProfile!,
                a => a.JobPosting)
                ?? throw new NotFoundException("JobApplication", request.JobApplicationId);

            var placeholderValues = BuildPlaceholderValues(jobApplication, request);
            var renderedBody = _placeholderSubstitutionService.Render(template.Body, placeholderValues);

            var pdfBytes = _offerLetterPdfGeneratorService.Generate(template.Name, jobApplication.CandidateName, renderedBody);

            using var pdfStream = new MemoryStream(pdfBytes);
            var (_, filePath) = await _fileStorageService.SaveAsync(pdfStream, "offer-letter.pdf", PdfStorageSubFolder);

            var entity = new OfferLetter
            {
                JobApplicationId = request.JobApplicationId,
                DocumentTemplateId = request.DocumentTemplateId,
                Designation = request.Designation,
                OfferedSalary = request.OfferedSalary,
                JoiningDate = request.JoiningDate,
                ReportingManager = request.ReportingManager,
                OfferValidityDate = request.OfferValidityDate,
                GeneratedPdfPath = filePath,
                Status = OfferLetterStatusEnum.Generated,
                GeneratedAt = DateTime.UtcNow,
            };

            await _offerLetterRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            entity.JobApplication = jobApplication;
            entity.DocumentTemplate = template;

            // EP-10 US-082 AC1: candidate gets a portal-link notification for the newly generated
            // offer. DispatchAsync never throws (a missing EventTemplateMapping just logs a
            // Skipped NotificationLog row), so this can't fail the generate call itself.
            await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.OfferLetterAvailable,
                placeholderValues,
                new NotificationDispatchTargets(jobApplication.CandidateEmail, null, jobApplication.JobApplicationId),
                persistImmediately: true);

            return entity.ToResponse();
        }

        public async Task<List<OfferLetterResponse>> GetAllAsync(long? jobApplicationId)
        {
            var entities = await _offerLetterRepository.GetAllOrderedAsync(jobApplicationId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<OfferLetterResponse> GetByIdAsync(long offerLetterId)
        {
            var entity = await _offerLetterRepository.GetByIdWithDetailsAsync(offerLetterId)
                ?? throw new NotFoundException("OfferLetter", offerLetterId);

            return entity.ToResponse();
        }

        public async Task<List<OfferLetterResponse>> GetAllForCandidateAsync()
        {
            var candidateProfileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entities = await _offerLetterRepository.GetAllForCandidateAsync(candidateProfileId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<OfferLetterResponse> GetByIdForCandidateAsync(long offerLetterId)
        {
            var entity = await GetOwnedOfferLetterAsync(offerLetterId);
            return entity.ToResponse();
        }

        public async Task<OfferLetterResponse> AcceptAsync(long offerLetterId)
        {
            var entity = await GetOwnedOfferLetterAsync(offerLetterId);
            EnsureUndecided(entity);

            entity.Status = OfferLetterStatusEnum.Accepted;
            entity.DecisionAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            await NotifyHrOfDecisionAsync(entity, RecruitmentEventEnum.OfferAccepted);
            return entity.ToResponse();
        }

        public async Task<OfferLetterResponse> DeclineAsync(long offerLetterId, string reason)
        {
            var entity = await GetOwnedOfferLetterAsync(offerLetterId);
            EnsureUndecided(entity);

            entity.Status = OfferLetterStatusEnum.Declined;
            entity.DecisionAt = DateTime.UtcNow;
            entity.DeclineReason = reason;
            await _unitOfWork.SaveChangesAsync();

            await NotifyHrOfDecisionAsync(entity, RecruitmentEventEnum.OfferDeclined);
            return entity.ToResponse();
        }

        // AC5: once a decision has been recorded it is locked - only Generated/Sent can still
        // transition. Ownership check mirrors ExamTakingService.GetOwnedEnrollmentAsync's shape.
        private async Task<OfferLetter> GetOwnedOfferLetterAsync(long offerLetterId)
        {
            var entity = await _offerLetterRepository.GetByIdWithDetailsAsync(offerLetterId)
                ?? throw new NotFoundException("OfferLetter", offerLetterId);

            var candidateProfileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            if (entity.JobApplication.CandidateProfileId != candidateProfileId)
                throw new ForbiddenException("This offer letter does not belong to you.");

            return entity;
        }

        private static void EnsureUndecided(OfferLetter entity)
        {
            if (entity.Status is not (OfferLetterStatusEnum.Generated or OfferLetterStatusEnum.Sent))
                throw new InvalidStatusTransitionException(nameof(OfferLetter), entity.Status, "a new decision");
        }

        private async Task NotifyHrOfDecisionAsync(OfferLetter entity, RecruitmentEventEnum recruitmentEvent)
        {
            var hrEmail = await _applicationSettingService.GetHrNotificationEmailAsync();
            var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = entity.JobApplication.CandidateName,
                ["Designation"] = entity.Designation,
                ["DecisionAt"] = entity.DecisionAt?.ToString("dd MMM yyyy HH:mm") ?? string.Empty,
                ["DeclineReason"] = entity.DeclineReason ?? string.Empty,
            };

            await _notificationDispatchService.DispatchAsync(
                recruitmentEvent,
                placeholders,
                new NotificationDispatchTargets(null, hrEmail, entity.JobApplicationId),
                persistImmediately: true);
        }

        private Dictionary<string, string> BuildPlaceholderValues(JobApplication jobApplication, OfferLetterGenerateRequest request)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = jobApplication.CandidateName,
                ["CandidateEmail"] = jobApplication.CandidateEmail ?? string.Empty,
                ["JobTitle"] = jobApplication.JobPosting?.Title ?? string.Empty,
                ["Designation"] = request.Designation,
                ["OfferedSalary"] = request.OfferedSalary.ToString("N2"),
                ["JoiningDate"] = request.JoiningDate.ToString("dd MMM yyyy"),
                ["ReportingManager"] = request.ReportingManager ?? string.Empty,
                ["OfferValidityDate"] = request.OfferValidityDate?.ToString("dd MMM yyyy") ?? string.Empty,
                ["PortalLink"] = $"{_portalSettings.FrontendBaseUrl}/candidate-profile/offer-letters",
            };
        }
    }
}
