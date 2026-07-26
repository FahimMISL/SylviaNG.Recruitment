using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

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
                ["SubmittedAt"] = submission.SubmittedAt?.ToString("dd MMM yyyy HH:mm") ?? string.Empty,
            };

            // Never throws - a missing EventTemplateMapping just logs a Skipped NotificationLog row.
            await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.PreBoardingSubmitted,
                placeholders,
                new NotificationDispatchTargets(null, hrEmail, pool.JobApplicationId),
                persistImmediately: true);

            return submission.ToResponse();
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

        private static void EnsureNotLocked(PreBoardingSubmission submission)
        {
            if (submission.Status != PreBoardingSubmissionStatusEnum.Draft)
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
