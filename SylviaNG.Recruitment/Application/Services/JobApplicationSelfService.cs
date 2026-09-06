using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Domain.Events;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JobApplicationSelfService : IJobApplicationSelfService
    {
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IJobApplicationStageProgressRepository _jobApplicationStageProgressRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<JobApplicationSelfService> _logger;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly IJobApplicationDispatchTargetBuilder _dispatchTargetBuilder;

        public JobApplicationSelfService(
            ICurrentCandidateService currentCandidateService,
            IJobApplicationRepository jobApplicationRepository,
            IJobApplicationStageProgressRepository jobApplicationStageProgressRepository,
            IJobPostingRepository jobPostingRepository,
            ICandidateProfileRepository candidateProfileRepository,
            IUnitOfWork unitOfWork,
            ILogger<JobApplicationSelfService> logger,
            INotificationDispatchService notificationDispatchService,
            IJobApplicationDispatchTargetBuilder dispatchTargetBuilder)
        {
            _currentCandidateService = currentCandidateService;
            _jobApplicationRepository = jobApplicationRepository;
            _jobApplicationStageProgressRepository = jobApplicationStageProgressRepository;
            _jobPostingRepository = jobPostingRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _notificationDispatchService = notificationDispatchService;
            _dispatchTargetBuilder = dispatchTargetBuilder;
        }

        public async Task<List<MyApplicationResponse>> GetMyApplicationsAsync()
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var email = await _currentCandidateService.GetCurrentEmailAsync();
            var applications = await _jobApplicationRepository.GetByCandidateAsync(profileId, email);

            var result = new List<MyApplicationResponse>();
            foreach (var a in applications)
            {
                var response = a.ToMyApplicationResponse(CanWithdraw(a.ApplicationStatus, a.JobPosting?.Status));

                // Merge in the generic pipeline-stage tracker's own schedule/meeting-link rows
                // (see ToMyApplicationInterviewResponse(JobApplicationStageProgress) doc) - a
                // second scheduling source alongside the dedicated Interview entity above.
                // Same actionable-only filter as the dedicated Interview list above - a Completed
                // stage's old ScheduledDate/MeetingLink is stale, not a second live meeting.
                var stageProgress = await _jobApplicationStageProgressRepository.GetByJobApplicationIdAsync(a.JobApplicationId);
                response.Interviews.AddRange(stageProgress
                    .Where(p => p.ScheduledDate.HasValue && p.Status != StageProgressStatusEnum.Completed)
                    .Select(p => p.ToMyApplicationInterviewResponse()));
                response.Interviews = response.Interviews.OrderBy(i => i.ScheduledDate).ToList();

                result.Add(response);
            }

            return result;
        }

        public async Task WithdrawMyApplicationAsync(long jobApplicationId)
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var email = await _currentCandidateService.GetCurrentEmailAsync();

            var entity = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            // Ownership check: linked profile match is authoritative; email match is the fallback
            // only for a row that predates the FK or that this candidate submitted as a guest
            // before registering (CandidateProfileId still null). Don't reveal that a
            // mismatched-owner application exists - report it as not found either way.
            var isOwner = entity.CandidateProfileId.HasValue
                ? entity.CandidateProfileId.Value == profileId
                : !string.IsNullOrEmpty(entity.CandidateEmail) && string.Equals(entity.CandidateEmail, email, StringComparison.OrdinalIgnoreCase);

            if (!isOwner)
            {
                throw new NotFoundException("JobApplication", jobApplicationId);
            }

            if (entity.ApplicationStatus == ApplicationStatusEnum.Withdrawn)
                return;

            JobApplicationStatusRules.EnsureLegalStatusTransition(entity.ApplicationStatus, ApplicationStatusEnum.Withdrawn);

            var jobPosting = await _jobPostingRepository.GetByIdAsync(entity.JobPostingId);
            if (jobPosting?.Status != JobStatusEnum.Open)
            {
                throw new InvalidStatusTransitionException(
                    "Application cannot be withdrawn because the job posting is no longer open.");
            }

            var fromStatus = entity.ApplicationStatus;
            entity.ApplicationStatus = ApplicationStatusEnum.Withdrawn;
            _jobApplicationRepository.Update(entity);

            entity.StatusHistory.Add(new ApplicationStatusHistory
            {
                JobApplicationId = entity.JobApplicationId,
                FromStatus = fromStatus,
                ToStatus = ApplicationStatusEnum.Withdrawn,
                ChangedByUserName = email,
                ChangedAt = DateTime.UtcNow,
                Note = "Withdrawn by candidate"
            });

            entity.AddDomainEvent(new ApplicationStatusChangedEvent
            {
                JobApplicationId = entity.JobApplicationId,
                FromStatus = fromStatus.ToString(),
                ToStatus = ApplicationStatusEnum.Withdrawn.ToString()
            });

            // US-075: separate hook from JobApplicationStatusService.ApplyStatusChangeAsync since
            // this path duplicates the status-history logic inline rather than calling it.
            try
            {
                var placeholders = JobApplicationDispatchTargetBuilder.BuildBasePlaceholders(entity, jobPosting?.Title ?? string.Empty);
                placeholders["FromStatus"] = fromStatus.ToString();
                placeholders["ToStatus"] = ApplicationStatusEnum.Withdrawn.ToString();

                await _notificationDispatchService.DispatchAsync(
                    RecruitmentEventEnum.ApplicationWithdrawn,
                    placeholders,
                    await _dispatchTargetBuilder.BuildTargetsAsync(entity),
                    persistImmediately: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error dispatching withdrawal notification for JobApplicationId {JobApplicationId}.", entity.JobApplicationId);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private static bool CanWithdraw(ApplicationStatusEnum currentStatus, JobStatusEnum? jobStatus)
        {
            return jobStatus == JobStatusEnum.Open
                && JobApplicationStatusRules.LegalStatusTransitions.TryGetValue(currentStatus, out var allowedTransitions)
                && allowedTransitions.Contains(ApplicationStatusEnum.Withdrawn);
        }

        public async Task<JobEligibilityResponse> CheckEligibilityAsync(long jobPostingId)
        {
            var jobPosting = await _jobPostingRepository.GetByIdAsync(jobPostingId)
                ?? throw new NotFoundException("JobPosting", jobPostingId);

            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var profile = await _candidateProfileRepository.GetByIdWithIncludeAsync(
                p => p.CandidateProfileId == profileId,
                p => p.Educations, p => p.WorkExperiences, p => p.Skills, p => p.PresentDistrict!, p => p.HomeDistrict!);

            var facts = CandidateFactService.BuildFacts(profile);
            var unmetRequirements = JobEligibilityEvaluator.Evaluate(jobPosting, facts);

            return new JobEligibilityResponse
            {
                IsEligible = unmetRequirements.Count == 0,
                UnmetRequirements = unmetRequirements
            };
        }
    }
}
