using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class FinalSelectionPoolService : IFinalSelectionPoolService
    {
        private readonly IFinalSelectionPoolRepository _finalSelectionPoolRepository;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IJobApplicationStageProgressService _jobApplicationStageProgressService;
        private readonly PortalSettings _portalSettings;
        private readonly IUnitOfWork _unitOfWork;

        private const string CandidateJoinedSystemActor = "system:candidate-joined";

        public FinalSelectionPoolService(
            IFinalSelectionPoolRepository finalSelectionPoolRepository,
            INotificationDispatchService notificationDispatchService,
            IApplicationSettingService applicationSettingService,
            IJobApplicationStageProgressService jobApplicationStageProgressService,
            IOptions<PortalSettings> portalSettings,
            IUnitOfWork unitOfWork)
        {
            _finalSelectionPoolRepository = finalSelectionPoolRepository;
            _notificationDispatchService = notificationDispatchService;
            _applicationSettingService = applicationSettingService;
            _jobApplicationStageProgressService = jobApplicationStageProgressService;
            _portalSettings = portalSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateFromAcceptedOfferAsync(OfferLetter offerLetter)
        {
            // EnsureUndecided in OfferLetterService already blocks a second Accept on the same
            // offer, and the unique index on OfferLetterId would reject a duplicate write anyway -
            // this check is defensive, not load-bearing.
            var existing = await _finalSelectionPoolRepository.GetByOfferLetterIdAsync(offerLetter.OfferLetterId);
            if (existing != null)
                return;

            var entity = new FinalSelectionPool
            {
                OfferLetterId = offerLetter.OfferLetterId,
                JobApplicationId = offerLetter.JobApplicationId,
                JoiningDate = offerLetter.JoiningDate,
                HasJoined = false,
                EnteredPoolAt = DateTime.UtcNow,
            };

            await _finalSelectionPoolRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = offerLetter.JobApplication.CandidateName,
                ["Designation"] = offerLetter.Designation,
                ["JoiningDate"] = entity.JoiningDate.ToString("dd MMM yyyy"),
                ["PortalLink"] = $"{_portalSettings.FrontendBaseUrl}/candidate-profile/pre-boarding",
            };

            // Never throws - a missing EventTemplateMapping just logs a Skipped NotificationLog row.
            await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.PreBoardingRequested,
                placeholders,
                new NotificationDispatchTargets(offerLetter.JobApplication.CandidateEmail, await _applicationSettingService.GetHrNotificationEmailAsync(), offerLetter.JobApplicationId),
                persistImmediately: true);
        }

        public async Task<List<FinalSelectionPoolResponse>> GetAllAsync()
        {
            var entities = await _finalSelectionPoolRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<FinalSelectionPoolResponse> GetByIdAsync(long finalSelectionPoolId)
        {
            var entity = await GetRequiredAsync(finalSelectionPoolId);
            return entity.ToResponse();
        }

        public async Task<FinalSelectionPoolResponse> MarkHasJoinedAsync(long finalSelectionPoolId)
        {
            var entity = await GetRequiredAsync(finalSelectionPoolId);

            if (entity.HasJoined)
                throw new InvalidStatusTransitionException(nameof(FinalSelectionPool), "HasJoined=true", "HasJoined=true");

            // Mirrors the frontend's disabled-button gate: joining formalities (Pre-Boarding) must
            // be HR-Approved first - candidate submitting isn't enough, HR has to have validated the
            // bank/nominee/emergency-contact details. Enforced here too so it can't be bypassed via
            // a direct API call.
            if (entity.PreBoardingSubmission?.Status != PreBoardingSubmissionStatusEnum.Approved)
                throw new InvalidStatusTransitionException(
                    "Pre-Boarding must be Approved before marking the candidate as joined.");

            entity.HasJoined = true;
            entity.JoinedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            // The pipeline's "Onboarding" stage card was previously a free-standing manual
            // checkbox with no link to whether the candidate actually joined (see FinalSelectionPool
            // vs JobApplicationStageProgress being entirely separate entities) - HR could mark it
            // Completed before Pre-Boarding was even submitted. Mirrors OfferLetterService.AcceptAsync's
            // AutoCompleteStageByTypeAsync("Offer", ...) call: the real-world event (candidate
            // physically joining) is now what drives the stage to Completed, not a manual click.
            await _jobApplicationStageProgressService.AutoCompleteStageByTypeAsync(
                entity.JobApplicationId, "Onboarding", 100m, CandidateJoinedSystemActor);

            return entity.ToResponse();
        }

        public async Task<FinalSelectionPoolResponse> UpdateBatchAsync(long finalSelectionPoolId, FinalSelectionPoolUpdateBatchRequest request)
        {
            var entity = await GetRequiredAsync(finalSelectionPoolId);

            entity.BatchLabel = request.BatchLabel;
            entity.JoiningDate = request.JoiningDate;
            await _unitOfWork.SaveChangesAsync();

            return entity.ToResponse();
        }

        private async Task<FinalSelectionPool> GetRequiredAsync(long finalSelectionPoolId)
        {
            return await _finalSelectionPoolRepository.GetByIdWithDetailsAsync(finalSelectionPoolId)
                ?? throw new NotFoundException("FinalSelectionPool", finalSelectionPoolId);
        }
    }
}
