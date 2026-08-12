using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Constants;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Utils;

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
        private readonly IFinalSelectionPoolService _finalSelectionPoolService;
        private readonly IJobApplicationStageProgressService _jobApplicationStageProgressService;
        private readonly PortalSettings _portalSettings;
        private readonly IUnitOfWork _unitOfWork;

        private const string PdfStorageSubFolder = "documents/offer-letters";
        private const string OfferAcceptedSystemActor = "system:offer-accepted";
        private const string OfferGeneratedSystemActor = "system:offer-generated";

        private static readonly ApplicationStatusEnum[] TerminalApplicationStatuses =
        {
            ApplicationStatusEnum.Hired,
            ApplicationStatusEnum.Rejected,
            ApplicationStatusEnum.Withdrawn,
            ApplicationStatusEnum.DuplicateDismissed,
        };

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
            IFinalSelectionPoolService finalSelectionPoolService,
            IJobApplicationStageProgressService jobApplicationStageProgressService,
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
            _finalSelectionPoolService = finalSelectionPoolService;
            _jobApplicationStageProgressService = jobApplicationStageProgressService;
            _portalSettings = portalSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<OfferLetterResponse> GenerateAsync(OfferLetterGenerateRequest request)
        {
            // An offer follows every evaluation stage being Completed - a passing candidate goes
            // straight to the offer, no separate recommend/approve step in between. Checked first,
            // ahead of template validation, since it's the more fundamental business-rule gate.
            var progress = await _jobApplicationStageProgressService.GetByJobApplicationIdAsync(request.JobApplicationId);
            if (progress.HasPipeline)
            {
                var incompleteRequiredStage = progress.Stages.FirstOrDefault(s =>
                    s.IsMandatory
                    && !PipelineStageTypes.PostDecision.Contains(s.StageType, StringComparer.OrdinalIgnoreCase)
                    && s.Status != StageProgressStatusEnum.Completed);

                if (incompleteRequiredStage != null)
                    throw new FluentValidation.ValidationException(new[]
                    {
                        new FluentValidation.Results.ValidationFailure(
                            nameof(request.JobApplicationId),
                            $"An offer letter can only be generated once all required stages are Completed ('{incompleteRequiredStage.StageName}' is not yet Completed).")
                    });
            }

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

            var pdfBytes = await _offerLetterPdfGeneratorService.Generate(template.Name, jobApplication.CandidateName, renderedBody, jobApplication.JobApplicationId);

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

            // Same ApplicationStatus/pipeline-stage split as AcceptAsync's Hired transition below -
            // generating the offer is the system event that should push the application to
            // Offered, since nothing else in the codebase writes that status automatically.
            await AutoTransitionApplicationStatusAsync(
                jobApplication, ApplicationStatusEnum.Offered, OfferGeneratedSystemActor,
                "Auto-transitioned: offer letter generated.");

            // EP-10 US-082 AC1: candidate gets a portal-link notification for the newly generated
            // offer. DispatchAsync never throws (a missing EventTemplateMapping just logs a
            // Skipped NotificationLog row), so this can't fail the generate call itself.
            await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.OfferLetterAvailable,
                placeholderValues,
                new NotificationDispatchTargets(jobApplication.CandidateEmail, await _applicationSettingService.GetHrNotificationEmailAsync(), jobApplication.JobApplicationId),
                persistImmediately: true);

            return entity.ToResponse();
        }

        public async Task<List<CandidateHireConflictResponse>> GetCandidateHireConflictsAsync(long jobApplicationId)
        {
            var application = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            var siblings = await _jobApplicationRepository.GetByCandidateAsync(application.CandidateProfileId, application.CandidateEmail!);

            return siblings
                .Where(a => a.JobApplicationId != jobApplicationId && a.ApplicationStatus == ApplicationStatusEnum.Hired)
                .Select(a => new CandidateHireConflictResponse
                {
                    JobApplicationId = a.JobApplicationId,
                    JobPostingTitle = a.JobPosting?.Title ?? string.Empty,
                    ApplicationStatus = a.ApplicationStatus.ToString(),
                })
                .ToList();
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

            // EP-12 US-094: entering the Final Selection Pool is the onboarding pipeline's entry
            // point - every Accepted offer gets exactly one pool row, created here so no other path
            // can accept an offer without also enrolling it in the pool.
            await _finalSelectionPoolService.CreateFromAcceptedOfferAsync(entity);

            // The "Offer" pipeline stage's own passing criteria is literally "Candidate accepts
            // the offer" - there's no meaningful numeric score for it, so 100 is used as a
            // pass/fail sentinel (also lets it participate in AutoProgressionTargetDisplayOrder
            // like any other stage, if a pipeline configures one out of Offer).
            await _jobApplicationStageProgressService.AutoCompleteStageByTypeAsync(
                entity.JobApplicationId, "Offer", 100m, OfferAcceptedSystemActor);

            // JobApplication.ApplicationStatus is a separate HR-facing field from the pipeline
            // stage above, and its dropdown only allows single-step manual moves (see
            // JobApplicationService.LegalStatusTransitions) - so without this, an accepted offer
            // leaves the application stuck wherever HR last set it (e.g. Shortlisted), with no
            // legal manual path to Hired. Mirrors PaymentService.HandleIpnAsync's direct write for
            // the same reason: a system event, not an HR pick, is driving this transition.
            await AutoTransitionApplicationStatusAsync(
                entity.JobApplication, ApplicationStatusEnum.Hired, OfferAcceptedSystemActor,
                "Auto-transitioned: candidate accepted the offer letter.");

            return entity.ToResponse();
        }

        // Same rationale as the Hired transition in AcceptAsync below - offer generation is a
        // system event (HR clicked "Generate", not "set status to Offered"), so it should push
        // ApplicationStatus forward too instead of leaving it wherever HR last manually set it.
        private async Task AutoTransitionApplicationStatusAsync(
            JobApplication? jobApplication, ApplicationStatusEnum toStatus, string systemActor, string note)
        {
            if (jobApplication == null
                || jobApplication.ApplicationStatus == toStatus
                || TerminalApplicationStatuses.Contains(jobApplication.ApplicationStatus))
                return;

            var fromStatus = jobApplication.ApplicationStatus;
            jobApplication.ApplicationStatus = toStatus;
            _jobApplicationRepository.Update(jobApplication);

            jobApplication.StatusHistory.Add(new ApplicationStatusHistory
            {
                JobApplicationId = jobApplication.JobApplicationId,
                FromStatus = fromStatus,
                ToStatus = toStatus,
                ChangedByUserName = systemActor,
                ChangedAt = DateTime.UtcNow,
                Note = note,
            });

            await _unitOfWork.SaveChangesAsync();
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
                ["DecisionAt"] = entity.DecisionAt.HasValue ? DateTimeUtility.ConvertUtcToLocal(entity.DecisionAt.Value).ToString("dd MMM yyyy hh:mm tt") : string.Empty,
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
