using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Application.Services
{
    public class PreBoardingService : IPreBoardingService
    {
        private readonly IFinalSelectionPoolRepository _finalSelectionPoolRepository;
        private readonly IPreBoardingSubmissionRepository _preBoardingSubmissionRepository;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IUnitOfWork _unitOfWork;

        public PreBoardingService(
            IFinalSelectionPoolRepository finalSelectionPoolRepository,
            IPreBoardingSubmissionRepository preBoardingSubmissionRepository,
            ICurrentCandidateService currentCandidateService,
            INotificationDispatchService notificationDispatchService,
            IApplicationSettingService applicationSettingService,
            IUnitOfWork unitOfWork)
        {
            _finalSelectionPoolRepository = finalSelectionPoolRepository;
            _preBoardingSubmissionRepository = preBoardingSubmissionRepository;
            _currentCandidateService = currentCandidateService;
            _notificationDispatchService = notificationDispatchService;
            _applicationSettingService = applicationSettingService;
            _unitOfWork = unitOfWork;
        }

        public async Task<PreBoardingSubmissionResponse> GetForCurrentCandidateAsync()
        {
            var (_, submission) = await GetOwnedPoolAndSubmissionAsync(autoCreateDraft: true);
            return submission!.ToResponse();
        }

        public async Task<bool> IsEligibleForCurrentCandidateAsync()
        {
            var candidateProfileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var pool = await _finalSelectionPoolRepository.GetByCandidateProfileIdWithDetailsAsync(candidateProfileId);
            return pool != null;
        }

        public async Task<PreBoardingSubmissionResponse> SaveDraftAsync(PreBoardingSaveRequest request)
        {
            var (_, submission) = await GetOwnedPoolAndSubmissionAsync(autoCreateDraft: true);

            EnsureNotLocked(submission!);

            submission!.EmergencyContactName = request.EmergencyContactName;
            submission.EmergencyContactRelationship = request.EmergencyContactRelationship;
            submission.EmergencyContactPhone = request.EmergencyContactPhone;
            submission.InsuranceProvider = request.InsuranceProvider;
            submission.InsurancePolicyNumber = request.InsurancePolicyNumber;
            submission.InsuranceNotes = request.InsuranceNotes;
            submission.BankName = request.BankName;
            submission.BankBranch = request.BankBranch;
            submission.BankAccountName = request.BankAccountName;
            submission.BankAccountNumber = request.BankAccountNumber;
            submission.BankRoutingNumber = request.BankRoutingNumber;

            // Full replace - simpler than per-item CRUD since the whole form saves as one unit.
            submission.Nominees.Clear();
            foreach (var nominee in request.Nominees)
            {
                submission.Nominees.Add(new PreBoardingNominee
                {
                    FullName = nominee.FullName,
                    Relationship = nominee.Relationship,
                    SharePercentage = nominee.SharePercentage,
                    ContactPhone = nominee.ContactPhone,
                    Address = nominee.Address,
                });
            }

            await _unitOfWork.SaveChangesAsync();
            return submission.ToResponse();
        }

        public async Task<PreBoardingSubmissionResponse> SubmitAsync()
        {
            var (pool, submission) = await GetOwnedPoolAndSubmissionAsync(autoCreateDraft: true);

            EnsureNotLocked(submission!);
            EnsureReadyToSubmit(submission!);

            submission!.Status = PreBoardingSubmissionStatusEnum.Submitted;
            submission.SubmittedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            var hrEmail = await _applicationSettingService.GetHrNotificationEmailAsync();
            var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = pool!.JobApplication.CandidateName,
                ["SubmittedAt"] = (submission.SubmittedAt.HasValue ? DateTimeUtility.ConvertUtcToLocal(submission.SubmittedAt.Value).ToString("dd MMM yyyy hh:mm tt") : string.Empty),
            };

            // Never throws - a missing EventTemplateMapping just logs a Skipped NotificationLog row.
            await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.PreBoardingSubmitted,
                placeholders,
                new NotificationDispatchTargets(null, hrEmail, pool.JobApplicationId),
                persistImmediately: true);

            return submission.ToResponse();
        }

        public async Task<PreBoardingSubmissionResponse> GetByFinalSelectionPoolIdForHrAsync(long finalSelectionPoolId)
        {
            var submission = await _preBoardingSubmissionRepository.GetByFinalSelectionPoolIdWithDetailsAsync(finalSelectionPoolId)
                ?? throw new NotFoundException("PreBoardingSubmission", finalSelectionPoolId);

            return submission.ToResponse();
        }

        public async Task<PreBoardingSubmissionResponse> ValidateAsync(long preBoardingSubmissionId)
        {
            var submission = await GetRequiredWithDetailsAsync(preBoardingSubmissionId);

            if (submission.Status != PreBoardingSubmissionStatusEnum.Submitted)
                throw new InvalidStatusTransitionException(nameof(PreBoardingSubmission), submission.Status, PreBoardingSubmissionStatusEnum.Approved);

            submission.Status = PreBoardingSubmissionStatusEnum.Approved;
            submission.CorrectionComment = null;
            await _unitOfWork.SaveChangesAsync();

            var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = submission.FinalSelectionPool.JobApplication.CandidateName,
                ["ValidatedAt"] = DateTimeUtility.ConvertUtcToLocal(DateTime.UtcNow).ToString("dd MMM yyyy hh:mm tt"),
            };

            // Never throws - a missing EventTemplateMapping just logs a Skipped NotificationLog row.
            await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.PreBoardingApproved,
                placeholders,
                new NotificationDispatchTargets(submission.FinalSelectionPool.JobApplication.CandidateEmail, await _applicationSettingService.GetHrNotificationEmailAsync(), submission.FinalSelectionPool.JobApplicationId),
                persistImmediately: true);

            return submission.ToResponse();
        }

        public async Task<PreBoardingSubmissionResponse> RequestCorrectionAsync(long preBoardingSubmissionId, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(nameof(comment), "A comment is required when requesting corrections."),
                });

            var submission = await GetRequiredWithDetailsAsync(preBoardingSubmissionId);

            // AC5: HR can reopen even an already-Approved submission if something's found later -
            // only Draft/NeedsCorrection (nothing submitted yet / already reopened) are illegal here.
            if (submission.Status is not (PreBoardingSubmissionStatusEnum.Submitted or PreBoardingSubmissionStatusEnum.Approved))
                throw new InvalidStatusTransitionException(nameof(PreBoardingSubmission), submission.Status, PreBoardingSubmissionStatusEnum.NeedsCorrection);

            submission.Status = PreBoardingSubmissionStatusEnum.NeedsCorrection;
            submission.CorrectionComment = comment;
            await _unitOfWork.SaveChangesAsync();

            var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = submission.FinalSelectionPool.JobApplication.CandidateName,
                ["Comment"] = comment,
            };

            // Never throws - a missing EventTemplateMapping just logs a Skipped NotificationLog row.
            await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.PreBoardingCorrectionRequested,
                placeholders,
                new NotificationDispatchTargets(submission.FinalSelectionPool.JobApplication.CandidateEmail, await _applicationSettingService.GetHrNotificationEmailAsync(), submission.FinalSelectionPool.JobApplicationId),
                persistImmediately: true);

            return submission.ToResponse();
        }

        private async Task<PreBoardingSubmission> GetRequiredWithDetailsAsync(long preBoardingSubmissionId)
        {
            return await _preBoardingSubmissionRepository.GetByIdWithDetailsAsync(preBoardingSubmissionId)
                ?? throw new NotFoundException("PreBoardingSubmission", preBoardingSubmissionId);
        }

        private async Task<(FinalSelectionPool? Pool, PreBoardingSubmission? Submission)> GetOwnedPoolAndSubmissionAsync(bool autoCreateDraft)
        {
            var candidateProfileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var pool = await _finalSelectionPoolRepository.GetByCandidateProfileIdWithDetailsAsync(candidateProfileId)
                ?? throw new NotFoundException("FinalSelectionPool", candidateProfileId);

            if (pool.PreBoardingSubmission != null)
                return (pool, pool.PreBoardingSubmission);

            if (!autoCreateDraft)
                return (pool, null);

            var submission = new PreBoardingSubmission
            {
                FinalSelectionPoolId = pool.FinalSelectionPoolId,
                Status = PreBoardingSubmissionStatusEnum.Draft,
            };

            await _preBoardingSubmissionRepository.AddAsync(submission);
            await _unitOfWork.SaveChangesAsync();

            pool.PreBoardingSubmission = submission;
            return (pool, submission);
        }

        // AC5: NeedsCorrection re-opens the form for candidate edits exactly like Draft - this is
        // the one check standing between HR's RequestCorrectionAsync and the candidate being able
        // to save/resubmit again.
        private static void EnsureNotLocked(PreBoardingSubmission submission)
        {
            if (submission.Status is not (PreBoardingSubmissionStatusEnum.Draft or PreBoardingSubmissionStatusEnum.NeedsCorrection))
                throw new InvalidStatusTransitionException(nameof(PreBoardingSubmission), submission.Status, "a draft edit");
        }

        private static void EnsureReadyToSubmit(PreBoardingSubmission submission)
        {
            var failures = new List<FluentValidation.Results.ValidationFailure>();

            if (string.IsNullOrWhiteSpace(submission.EmergencyContactName)
                || string.IsNullOrWhiteSpace(submission.EmergencyContactRelationship)
                || string.IsNullOrWhiteSpace(submission.EmergencyContactPhone))
            {
                failures.Add(new FluentValidation.Results.ValidationFailure(
                    nameof(submission.EmergencyContactName), "Emergency contact must be fully completed before submitting."));
            }

            if (string.IsNullOrWhiteSpace(submission.BankName)
                || string.IsNullOrWhiteSpace(submission.BankAccountName)
                || string.IsNullOrWhiteSpace(submission.BankAccountNumber))
            {
                failures.Add(new FluentValidation.Results.ValidationFailure(
                    nameof(submission.BankName), "Bank details must be fully completed before submitting."));
            }

            if (submission.Nominees.Count == 0)
            {
                failures.Add(new FluentValidation.Results.ValidationFailure(
                    nameof(submission.Nominees), "At least one nominee is required before submitting."));
            }
            else if (submission.Nominees.Sum(n => n.SharePercentage) != 100m)
            {
                failures.Add(new FluentValidation.Results.ValidationFailure(
                    nameof(submission.Nominees), "Nominee share percentages must sum to 100."));
            }

            if (failures.Count > 0)
                throw new FluentValidation.ValidationException(failures);
        }
    }
}
