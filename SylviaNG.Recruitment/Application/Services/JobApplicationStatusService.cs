using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Utilities;
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
    public class JobApplicationStatusService : IJobApplicationStatusService
    {
        private static readonly ApplicationStatusEnum[] StatusesRequiringReason = { ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn };

        // US-101: batches above this size go through the EP-13 F1 async export queue
        // (IExportRequestService.RequestBulkCvZipExportAsync) instead - QuestPDF rendering per
        // candidate is heavier than the cheap row-append bulk-status/bulk-notify do, so the
        // sync-download cap is intentionally much lower than BulkNotifyAsync's 500.
        public const int BulkDownloadCvsSyncMaxCount = 20;

        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IApplicationStatusReasonRepository _applicationStatusReasonRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly ICvPdfGeneratorService _cvPdfGeneratorService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<JobApplicationStatusService> _logger;
        private readonly IJobApplicationDispatchTargetBuilder _dispatchTargetBuilder;

        public JobApplicationStatusService(
            IJobApplicationRepository jobApplicationRepository,
            IJobPostingRepository jobPostingRepository,
            IApplicationStatusReasonRepository applicationStatusReasonRepository,
            ICurrentUserService currentUserService,
            INotificationDispatchService notificationDispatchService,
            ICandidateProfileRepository candidateProfileRepository,
            ICvPdfGeneratorService cvPdfGeneratorService,
            IUnitOfWork unitOfWork,
            ILogger<JobApplicationStatusService> logger,
            IJobApplicationDispatchTargetBuilder dispatchTargetBuilder)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobPostingRepository = jobPostingRepository;
            _applicationStatusReasonRepository = applicationStatusReasonRepository;
            _currentUserService = currentUserService;
            _notificationDispatchService = notificationDispatchService;
            _candidateProfileRepository = candidateProfileRepository;
            _cvPdfGeneratorService = cvPdfGeneratorService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _dispatchTargetBuilder = dispatchTargetBuilder;
        }

        public async Task<List<ApplicationStatusReasonResponse>> GetStatusReasonsAsync(ApplicationStatusEnum status)
        {
            var entities = await _applicationStatusReasonRepository.GetActiveByStatusAsync(status);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task UpdateStatusAsync(long jobApplicationId, JobApplicationStatusUpdateRequest request)
        {
            var entity = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            await ApplyStatusChangeAsync(entity, request);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<JobApplicationBulkStatusUpdateResponse> BulkUpdateStatusAsync(JobApplicationBulkStatusUpdateRequest request)
        {
            var result = new JobApplicationBulkStatusUpdateResponse();

            foreach (var jobApplicationId in request.JobApplicationIds)
            {
                try
                {
                    var entity = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                        ?? throw new NotFoundException("JobApplication", jobApplicationId);

                    await ApplyStatusChangeAsync(entity, new JobApplicationStatusUpdateRequest
                    {
                        ToStatus = request.ToStatus,
                        ReasonId = request.ReasonId,
                        Note = request.Note
                    });

                    result.SucceededIds.Add(jobApplicationId);
                }
                catch (Exception ex) when (ex is NotFoundException or InvalidStatusTransitionException or FluentValidation.ValidationException)
                {
                    result.Failed.Add(new JobApplicationBulkStatusUpdateFailure
                    {
                        JobApplicationId = jobApplicationId,
                        Reason = ex.Message
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        // US-076: same best-effort bulk shape as BulkUpdateStatusAsync above - a bad id shouldn't
        // block the rest of the batch. Unlike a status move, "notify" is idempotent and doesn't
        // touch ApplicationStatus, so there's no legal-transition check here.
        public async Task<JobApplicationBulkNotifyResponse> BulkNotifyAsync(JobApplicationBulkNotifyRequest request)
        {
            if (request.JobApplicationIds.Count == 0)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.JobApplicationIds), "At least one JobApplicationId is required.")
                });
            }

            if (request.JobApplicationIds.Count > 500)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.JobApplicationIds), "A bulk notify batch cannot exceed 500 applications.")
                });
            }

            var result = new JobApplicationBulkNotifyResponse();

            foreach (var jobApplicationId in request.JobApplicationIds)
            {
                try
                {
                    var entity = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                        ?? throw new NotFoundException("JobApplication", jobApplicationId);

                    var jobPosting = await _jobPostingRepository.GetByIdAsync(entity.JobPostingId);
                    var placeholders = JobApplicationDispatchTargetBuilder.BuildBasePlaceholders(entity, jobPosting?.Title ?? string.Empty);

                    await _notificationDispatchService.DispatchAsync(
                        request.RecruitmentEvent,
                        placeholders,
                        await _dispatchTargetBuilder.BuildTargetsAsync(entity),
                        persistImmediately: false);

                    result.SucceededIds.Add(jobApplicationId);
                }
                catch (NotFoundException ex)
                {
                    result.Failed.Add(new JobApplicationBulkNotifyFailure
                    {
                        JobApplicationId = jobApplicationId,
                        Reason = ex.Message
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<JobApplicationCvBulkDownloadResponse> BulkDownloadCvsAsync(JobApplicationCvBulkDownloadRequest request)
        {
            var jobApplicationIds = request.JobApplicationIds.Distinct().ToList();

            if (jobApplicationIds.Count == 0)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.JobApplicationIds), "Select at least one candidate.")
                });
            }

            if (jobApplicationIds.Count > BulkDownloadCvsSyncMaxCount)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.JobApplicationIds),
                        $"A synchronous bulk CV download cannot exceed {BulkDownloadCvsSyncMaxCount} applications - use the Export Requests queue for larger batches.")
                });
            }

            var applications = _jobApplicationRepository.Query()
                .Where(a => jobApplicationIds.Contains(a.JobApplicationId))
                .ToList();

            var content = await BuildCvZipAsync(applications, CancellationToken.None);

            return new JobApplicationCvBulkDownloadResponse
            {
                Content = content,
                ContentType = "application/zip",
                FileName = $"Candidate-CVs-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip"
            };
        }

        private async Task<byte[]> BuildCvZipAsync(List<JobApplication> applications, CancellationToken cancellationToken)
        {
            var profileIds = applications
                .Where(a => a.CandidateProfileId.HasValue)
                .Select(a => a.CandidateProfileId!.Value)
                .Distinct()
                .ToList();

            var profilesById = profileIds.Count == 0
                ? new Dictionary<long, CandidateProfile>()
                : (await _candidateProfileRepository.GetByIdsWithDetailsAsync(profileIds)).ToDictionary(p => p.CandidateProfileId);

            // Guest applicants with no CandidateProfileId have no system-rendered CV to include -
            // silently skipped, consistent with US-101 AC5 ("only CVs accessible to permission level").
            var items = applications
                .Where(a => a.CandidateProfileId.HasValue && profilesById.ContainsKey(a.CandidateProfileId.Value))
                .Select(a => (a.JobApplicationId, a.CandidateName, profilesById[a.CandidateProfileId!.Value]));

            return await CvZipBuilder.BuildAsync(items, _cvPdfGeneratorService, cancellationToken);
        }

        public async Task ApplyStatusChangeAsync(JobApplication entity, JobApplicationStatusUpdateRequest request)
        {
            var fromStatus = entity.ApplicationStatus;

            if (fromStatus != request.ToStatus)
                JobApplicationStatusRules.EnsureLegalStatusTransition(fromStatus, request.ToStatus);

            if (StatusesRequiringReason.Contains(request.ToStatus) && request.ReasonId == null)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.ReasonId),
                        $"A reason is required when moving an application to {request.ToStatus}.")
                });
            }

            entity.ApplicationStatus = request.ToStatus;
            _jobApplicationRepository.Update(entity);

            var history = new ApplicationStatusHistory
            {
                JobApplicationId = entity.JobApplicationId,
                FromStatus = fromStatus,
                ToStatus = request.ToStatus,
                ChangedByUserName = _currentUserService.GetCurrentUserName(),
                ChangedAt = DateTime.UtcNow,
                ReasonId = request.ReasonId,
                Note = request.Note
            };
            entity.StatusHistory.Add(history);

            entity.AddDomainEvent(new ApplicationStatusChangedEvent
            {
                JobApplicationId = entity.JobApplicationId,
                FromStatus = fromStatus.ToString(),
                ToStatus = request.ToStatus.ToString()
            });

            // US-075: covers both UpdateStatusAsync (single) and BulkUpdateStatusAsync (loop) -
            // both funnel through here. persistImmediately:false so the NotificationLog row rides
            // along in the caller's own SaveChangesAsync instead of committing early.
            try
            {
                var jobPosting = await _jobPostingRepository.GetByIdAsync(entity.JobPostingId);
                var placeholders = JobApplicationDispatchTargetBuilder.BuildBasePlaceholders(entity, jobPosting?.Title ?? string.Empty);
                placeholders["FromStatus"] = fromStatus.ToString();
                placeholders["ToStatus"] = request.ToStatus.ToString();

                // Rejection gets its own event so a dedicated rejection template can be mapped
                // separately; every other transition uses the generic status-changed event.
                var dispatchEvent = request.ToStatus == ApplicationStatusEnum.Rejected
                    ? RecruitmentEventEnum.ApplicationRejected
                    : RecruitmentEventEnum.ApplicationStatusChanged;

                await _notificationDispatchService.DispatchAsync(
                    dispatchEvent,
                    placeholders,
                    await _dispatchTargetBuilder.BuildTargetsAsync(entity),
                    persistImmediately: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error dispatching status-change notification for JobApplicationId {JobApplicationId}.", entity.JobApplicationId);
            }

            if (request.ToStatus == ApplicationStatusEnum.Hired)
                await MarkCandidateInternalAsync(entity);
        }

        // Hiring confirmation implies the person is now internal - auto-flag their candidate
        // profile the same way an HR/Admin manual override does (CandidateProfile.IsInternal).
        // Prefers the linked CandidateProfileId; falls back to email match only for rows that
        // predate the FK or that a guest submitted before registering.
        private async Task MarkCandidateInternalAsync(JobApplication entity)
        {
            CandidateProfile? profile = null;

            if (entity.CandidateProfileId.HasValue)
                profile = await _candidateProfileRepository.GetByIdAsync(entity.CandidateProfileId.Value);

            if (profile == null && !string.IsNullOrEmpty(entity.CandidateEmail))
            {
                var profiles = await _candidateProfileRepository.GetByEmailsAsync(new[] { entity.CandidateEmail });
                profile = profiles.FirstOrDefault();
            }

            if (profile == null || profile.IsManuallyInternal)
                return;

            profile.IsManuallyInternal = true;
            _candidateProfileRepository.Update(profile);
        }
    }
}
