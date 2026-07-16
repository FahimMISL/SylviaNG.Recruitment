using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Domain.Events;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IApplicationCvStorageService _applicationCvStorageService;
        private readonly IApplicationStatusReasonRepository _applicationStatusReasonRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IUnitOfWork _unitOfWork;

        private static readonly CircularTypeEnum[] ExternalCircularTypes = { CircularTypeEnum.ExternalOnly, CircularTypeEnum.Both };
        private static readonly CircularTypeEnum[] InternalCircularTypes = { CircularTypeEnum.InternalOnly, CircularTypeEnum.Both };

        // US-034 AC1: HR applying on a candidate's behalf can target any open vacancy, regardless
        // of its audience restriction.
        private static readonly CircularTypeEnum[] AdminCircularTypes = { CircularTypeEnum.ExternalOnly, CircularTypeEnum.InternalOnly, CircularTypeEnum.Both };

        // US-036 AC1: legal application status transitions. Reject/withdraw is reachable from any
        // active stage (not just the end of the pipeline); Hired/Rejected/Withdrawn are terminal.
        // Applied->Shortlisted is legal directly (not just via Screening) so US-044's automated
        // filter apply can fast-track a fresh application without a manual screening bump first -
        // that's the whole point of "without manual screening of every application" in US-043.
        private static readonly Dictionary<ApplicationStatusEnum, ApplicationStatusEnum[]> LegalStatusTransitions = new()
        {
            [ApplicationStatusEnum.Applied] = new[] { ApplicationStatusEnum.Screening, ApplicationStatusEnum.Shortlisted, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn },
            [ApplicationStatusEnum.Screening] = new[] { ApplicationStatusEnum.Shortlisted, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn },
            [ApplicationStatusEnum.Shortlisted] = new[] { ApplicationStatusEnum.InterviewScheduled, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn },
            [ApplicationStatusEnum.InterviewScheduled] = new[] { ApplicationStatusEnum.Interviewed, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn },
            [ApplicationStatusEnum.Interviewed] = new[] { ApplicationStatusEnum.Offered, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn },
            [ApplicationStatusEnum.Offered] = new[] { ApplicationStatusEnum.Hired, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn },
            [ApplicationStatusEnum.Hired] = Array.Empty<ApplicationStatusEnum>(),
            [ApplicationStatusEnum.Rejected] = Array.Empty<ApplicationStatusEnum>(),
            [ApplicationStatusEnum.Withdrawn] = Array.Empty<ApplicationStatusEnum>()
        };

        private static readonly ApplicationStatusEnum[] StatusesRequiringReason = { ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn };

        public JobApplicationService(
            IJobApplicationRepository jobApplicationRepository,
            IJobPostingRepository jobPostingRepository,
            ICandidateProfileRepository candidateProfileRepository,
            IApplicationCvStorageService applicationCvStorageService,
            IApplicationStatusReasonRepository applicationStatusReasonRepository,
            ICurrentUserService currentUserService,
            ICurrentCandidateService currentCandidateService,
            IUnitOfWork unitOfWork)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobPostingRepository = jobPostingRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _applicationCvStorageService = applicationCvStorageService;
            _applicationStatusReasonRepository = applicationStatusReasonRepository;
            _currentUserService = currentUserService;
            _currentCandidateService = currentCandidateService;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(JobApplicationCreateRequest request)
        {
            var jobPosting = await _jobPostingRepository.GetByIdAsync(request.JobPostingId)
                ?? throw new NotFoundException("JobPosting", request.JobPostingId);

            if (!string.IsNullOrEmpty(request.CandidateEmail))
            {
                var exists = await _jobApplicationRepository.GetByEmailAndJobPostingIdAsync(request.CandidateEmail, request.JobPostingId);
                if (exists != null)
                    throw new DuplicateException("JobApplication", "CandidateEmail", request.CandidateEmail);
            }

            var entity = request.ToEntity();
            entity.ApplicationStatus = ApplicationStatusEnum.Applied;
            entity.AppliedDate = DateTime.UtcNow;

            await _jobApplicationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.JobApplicationId;
        }

        public async Task UpdateAsync(long jobApplicationId, JobApplicationUpdateRequest request)
        {
            var entity = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            entity.ApplyUpdate(request);
            _jobApplicationRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long jobApplicationId)
        {
            var entity = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            _jobApplicationRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<JobApplicationResponse> GetByIdAsync(long jobApplicationId)
        {
            var entity = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == jobApplicationId,
                a => a.JobPosting)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<JobApplicationResponse>> GetPaginatedByJobPostingAsync(long jobPostingId, PagedRequest request)
        {
            var pagedResult = await _jobApplicationRepository.GetPaginatedByJobPostingAsync(jobPostingId, request);

            return new PagedResult<JobApplicationResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<JobApplicationResponse> SubmitAsync(JobApplicationSubmitRequest request, ApplicationSourceEnum source)
        {
            var allowedCircularTypes = source switch
            {
                ApplicationSourceEnum.Internal => InternalCircularTypes,
                ApplicationSourceEnum.Admin => AdminCircularTypes,
                _ => ExternalCircularTypes
            };

            var jobPosting = await _jobPostingRepository.GetOpenByIdAndCircularTypesAsync(request.JobPostingId, allowedCircularTypes)
                ?? throw new NotFoundException("JobPosting", request.JobPostingId);

            if (!string.IsNullOrEmpty(request.CandidateEmail))
            {
                var exists = await _jobApplicationRepository.GetByEmailAndJobPostingIdAsync(request.CandidateEmail, request.JobPostingId);
                if (exists != null)
                    throw new DuplicateException("JobApplication", "CandidateEmail", request.CandidateEmail);
            }

            var entity = request.ToEntity();
            entity.Source = source;
            entity.ApplicationStatus = ApplicationStatusEnum.Applied;
            entity.AppliedDate = DateTime.UtcNow;

            if (request.Resume != null)
            {
                var (_, filePath) = await _applicationCvStorageService.SaveAsync(
                    request.Resume.OpenReadStream(), request.Resume.FileName, jobPosting.JobPostingId.ToString());
                entity.ResumeUrl = filePath;
            }

            await _jobApplicationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            if (source == ApplicationSourceEnum.Admin && !string.IsNullOrEmpty(request.CandidateEmail))
            {
                entity.AddDomainEvent(new JobApplicationSubmittedOnBehalfEvent
                {
                    JobApplicationId = entity.JobApplicationId,
                    CandidateEmail = request.CandidateEmail
                });
            }

            entity.JobPosting = jobPosting;
            return entity.ToResponse();
        }

        // ── ATS Dashboard / Status Update (US-035 / US-036) ───────────────

        public async Task<PagedResult<JobApplicationDashboardResponse>> GetDashboardPagedAsync(
            PagedRequest request,
            JobApplicationAttributeFilterRequest filter)
        {
            ValidateAttributeFilterRequest(filter);

            if (!filter.HasCandidateAttributeFilters)
            {
                var pagedResult = await _jobApplicationRepository.GetPaginatedAllAsync(
                    request, filter.JobPostingId, filter.Status, filter.Source, filter.DateFrom, filter.DateTo);

                return new PagedResult<JobApplicationDashboardResponse>
                {
                    Data = pagedResult.Data.Select(e => e.ToDashboardResponse()).ToList(),
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }

            // Candidate-attribute filters require an in-memory join (email->profile, no FK), so
            // pagination happens after filtering here rather than in SQL - same tradeoff already
            // accepted by ShortlistFilterEvaluationService for single-vacancy datasets. Sorting is
            // fixed to AppliedDate desc on this path (column sort isn't supported alongside
            // candidate-attribute filters).
            var matched = await GetAttributeFilteredApplicationsAsync(filter);
            var page = matched
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResult<JobApplicationDashboardResponse>
            {
                Data = page.Select(e => e.ToDashboardResponse()).ToList(),
                TotalCount = matched.Count,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<JobApplicationDetailResponse> GetDetailAsync(long jobApplicationId)
        {
            var entity = await _jobApplicationRepository.GetByIdWithHistoryAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            return entity.ToDetailResponse();
        }

        public async Task<List<long>> GetDashboardMatchingIdsAsync(JobApplicationAttributeFilterRequest filter)
        {
            ValidateAttributeFilterRequest(filter);

            if (!filter.HasCandidateAttributeFilters)
                return await _jobApplicationRepository.GetAllMatchingIdsAsync(filter.JobPostingId, filter.Status, filter.Source, filter.DateFrom, filter.DateTo);

            var matched = await GetAttributeFilteredApplicationsAsync(filter);
            return matched.Select(a => a.JobApplicationId).ToList();
        }

        private static void ValidateAttributeFilterRequest(JobApplicationAttributeFilterRequest filter)
        {
            if (filter.HasCandidateAttributeFilters && !filter.JobPostingId.HasValue)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(filter.JobPostingId),
                        "JobPostingId is required when filtering by education, experience, skills, location, or age.")
                });
            }
        }

        private async Task<List<JobApplication>> GetAttributeFilteredApplicationsAsync(JobApplicationAttributeFilterRequest filter)
        {
            var applications = await _jobApplicationRepository.GetAllByJobPostingAndScalarFiltersAsync(
                filter.JobPostingId!.Value, filter.Status, filter.Source, filter.DateFrom, filter.DateTo);

            var emails = applications
                .Select(a => a.CandidateEmail)
                .Where(e => !string.IsNullOrEmpty(e))
                .Distinct()
                .Cast<string>()
                .ToList();

            var profiles = await _candidateProfileRepository.GetByEmailsAsync(emails);
            var profilesByEmail = profiles.ToDictionary(p => p.Email, p => p, StringComparer.OrdinalIgnoreCase);

            var matched = new List<JobApplication>();
            foreach (var application in applications)
            {
                CandidateProfile? profile = null;
                if (!string.IsNullOrEmpty(application.CandidateEmail))
                    profilesByEmail.TryGetValue(application.CandidateEmail, out profile);

                if (profile == null)
                    continue;

                var facts = CandidateFactService.BuildFacts(profile);
                if (MatchesAttributeFilter(facts, filter))
                    matched.Add(application);
            }

            return matched.OrderByDescending(a => a.AppliedDate).ToList();
        }

        public static bool MatchesAttributeFilter(CandidateFactService.CandidateFacts facts, JobApplicationAttributeFilterRequest filter)
        {
            if (filter.MinEducationLevel.HasValue
                && !facts.EducationLevels.Any(l => (int)l >= (int)filter.MinEducationLevel.Value))
                return false;

            if (filter.MinExperienceYears.HasValue && facts.TotalExperienceYears < (double)filter.MinExperienceYears.Value)
                return false;

            if (filter.MaxExperienceYears.HasValue && facts.TotalExperienceYears > (double)filter.MaxExperienceYears.Value)
                return false;

            if (filter.Skills != null && filter.Skills.Count > 0
                && !filter.Skills.Any(skill => facts.SkillNames.Contains(skill)))
                return false;

            if (!string.IsNullOrWhiteSpace(filter.Location)
                && !facts.AddressText.Contains(filter.Location, StringComparison.OrdinalIgnoreCase))
                return false;

            if (filter.MinAge.HasValue && (!facts.Age.HasValue || facts.Age.Value < filter.MinAge.Value))
                return false;

            if (filter.MaxAge.HasValue && (!facts.Age.HasValue || facts.Age.Value > filter.MaxAge.Value))
                return false;

            if (filter.Tags != null && filter.Tags.Count > 0
                && !filter.Tags.Any(tag => facts.TagNames.Contains(tag)))
                return false;

            return true;
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

        private async Task ApplyStatusChangeAsync(JobApplication entity, JobApplicationStatusUpdateRequest request)
        {
            var fromStatus = entity.ApplicationStatus;

            if (fromStatus != request.ToStatus)
                EnsureLegalStatusTransition(fromStatus, request.ToStatus);

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
        }

        private static void EnsureLegalStatusTransition(ApplicationStatusEnum currentStatus, ApplicationStatusEnum requestedStatus)
        {
            if (!LegalStatusTransitions.TryGetValue(currentStatus, out var allowedTransitions)
                || !allowedTransitions.Contains(requestedStatus))
            {
                throw new InvalidStatusTransitionException("JobApplication", currentStatus, requestedStatus);
            }
        }

        // ── Candidate Self-Service (US-040) ───────────────────────────────

        public async Task<List<MyApplicationResponse>> GetMyApplicationsAsync()
        {
            var email = await _currentCandidateService.GetCurrentEmailAsync();
            var applications = await _jobApplicationRepository.GetByCandidateEmailAsync(email);

            return applications
                .Select(a => a.ToMyApplicationResponse(CanWithdraw(a.ApplicationStatus)))
                .ToList();
        }

        public async Task WithdrawMyApplicationAsync(long jobApplicationId)
        {
            var email = await _currentCandidateService.GetCurrentEmailAsync();

            var entity = await _jobApplicationRepository.GetByIdAsync(jobApplicationId)
                ?? throw new NotFoundException("JobApplication", jobApplicationId);

            // Don't reveal that a mismatched-owner application exists - report it as not found,
            // same as any other candidate-scoped lookup.
            if (string.IsNullOrEmpty(entity.CandidateEmail)
                || !string.Equals(entity.CandidateEmail, email, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotFoundException("JobApplication", jobApplicationId);
            }

            if (entity.ApplicationStatus == ApplicationStatusEnum.Withdrawn)
                return;

            EnsureLegalStatusTransition(entity.ApplicationStatus, ApplicationStatusEnum.Withdrawn);

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

            await _unitOfWork.SaveChangesAsync();
        }

        private static bool CanWithdraw(ApplicationStatusEnum currentStatus)
        {
            return LegalStatusTransitions.TryGetValue(currentStatus, out var allowedTransitions)
                && allowedTransitions.Contains(ApplicationStatusEnum.Withdrawn);
        }
    }
}
