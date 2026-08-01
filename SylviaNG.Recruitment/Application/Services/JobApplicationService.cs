using Microsoft.AspNetCore.Http;
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
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private static readonly HashSet<string> ExtractableResumeExtensions = new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".docx" };

        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IJobApplicationStageProgressRepository _jobApplicationStageProgressRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly ICandidateDocumentRepository _candidateDocumentRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IApplicationCvStorageService _applicationCvStorageService;
        private readonly ICvPdfGeneratorService _cvPdfGeneratorService;
        private readonly IApplicationStatusReasonRepository _applicationStatusReasonRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IPaymentService _paymentService;
        private readonly IWaiverRuleService _waiverRuleService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IResumeParsingService _resumeParsingService;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<JobApplicationService> _logger;

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
        // US-038 AC3/AC4: DuplicateDismissed is reachable from any active stage, same as Withdrawn -
        // HR can resolve a duplicate at whatever point in the pipeline it's noticed.
        private static readonly Dictionary<ApplicationStatusEnum, ApplicationStatusEnum[]> LegalStatusTransitions = new()
        {
            [ApplicationStatusEnum.Applied] = new[] { ApplicationStatusEnum.Screening, ApplicationStatusEnum.Shortlisted, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn, ApplicationStatusEnum.DuplicateDismissed },
            [ApplicationStatusEnum.Screening] = new[] { ApplicationStatusEnum.Shortlisted, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn, ApplicationStatusEnum.DuplicateDismissed },
            [ApplicationStatusEnum.Shortlisted] = new[] { ApplicationStatusEnum.InterviewScheduled, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn, ApplicationStatusEnum.DuplicateDismissed },
            [ApplicationStatusEnum.InterviewScheduled] = new[] { ApplicationStatusEnum.Interviewed, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn, ApplicationStatusEnum.DuplicateDismissed },
            [ApplicationStatusEnum.Interviewed] = new[] { ApplicationStatusEnum.Offered, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn, ApplicationStatusEnum.DuplicateDismissed },
            [ApplicationStatusEnum.Offered] = new[] { ApplicationStatusEnum.Hired, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn, ApplicationStatusEnum.DuplicateDismissed },
            [ApplicationStatusEnum.Hired] = Array.Empty<ApplicationStatusEnum>(),
            [ApplicationStatusEnum.Rejected] = Array.Empty<ApplicationStatusEnum>(),
            [ApplicationStatusEnum.Withdrawn] = Array.Empty<ApplicationStatusEnum>(),

            // EP-17: Applied = payment confirmed via SSLCommerz IPN (PaymentService writes this
            // transition directly, bypassing this HR-user-scoped path - see PaymentService.HandleIpnAsync).
            // Rejected lets HR close out a stale/abandoned unpaid application; Withdrawn lets a
            // candidate with an account cancel one they never intend to pay.
            [ApplicationStatusEnum.AwaitingPayment] = new[] { ApplicationStatusEnum.Applied, ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn },
            [ApplicationStatusEnum.DuplicateDismissed] = Array.Empty<ApplicationStatusEnum>()
        };

        private static readonly ApplicationStatusEnum[] StatusesRequiringReason = { ApplicationStatusEnum.Rejected, ApplicationStatusEnum.Withdrawn };

        private const string WaiverSystemActor = "system:fee-waiver";

        public JobApplicationService(
            IJobApplicationRepository jobApplicationRepository,
            IJobApplicationStageProgressRepository jobApplicationStageProgressRepository,
            IJobPostingRepository jobPostingRepository,
            ICandidateProfileRepository candidateProfileRepository,
            ICandidateDocumentRepository candidateDocumentRepository,
            IFileStorageService fileStorageService,
            IApplicationCvStorageService applicationCvStorageService,
            ICvPdfGeneratorService cvPdfGeneratorService,
            IApplicationStatusReasonRepository applicationStatusReasonRepository,
            ICurrentUserService currentUserService,
            ICurrentCandidateService currentCandidateService,
            IPaymentService paymentService,
            IWaiverRuleService waiverRuleService,
            IApplicationSettingService applicationSettingService,
            IResumeParsingService resumeParsingService,
            INotificationDispatchService notificationDispatchService,
            IUnitOfWork unitOfWork,
            ILogger<JobApplicationService> logger)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobApplicationStageProgressRepository = jobApplicationStageProgressRepository;
            _jobPostingRepository = jobPostingRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _candidateDocumentRepository = candidateDocumentRepository;
            _fileStorageService = fileStorageService;
            _applicationCvStorageService = applicationCvStorageService;
            _cvPdfGeneratorService = cvPdfGeneratorService;
            _applicationStatusReasonRepository = applicationStatusReasonRepository;
            _currentUserService = currentUserService;
            _currentCandidateService = currentCandidateService;
            _paymentService = paymentService;
            _waiverRuleService = waiverRuleService;
            _applicationSettingService = applicationSettingService;
            _resumeParsingService = resumeParsingService;
            _notificationDispatchService = notificationDispatchService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private async Task<NotificationDispatchTargets> BuildDispatchTargetsAsync(JobApplication entity)
        {
            var hrEmail = await _applicationSettingService.GetHrNotificationEmailAsync();
            return new NotificationDispatchTargets(entity.CandidateEmail, hrEmail, entity.JobApplicationId);
        }

        private static Dictionary<string, string> BuildBaseDispatchPlaceholders(JobApplication entity, string jobPostingTitle)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = entity.CandidateName,
                ["JobPostingTitle"] = jobPostingTitle,
                ["ApplicationStatus"] = entity.ApplicationStatus.ToString()
            };
        }

        public async Task<long> CreateAsync(JobApplicationCreateRequest request, long? candidateProfileId = null)
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
            entity.CandidateProfileId = candidateProfileId ?? await ResolveCandidateProfileIdAsync(request.CandidateEmail);

            await _jobApplicationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.JobApplicationId;
        }

        // Resolves the applicant's CandidateProfile at submission time instead of leaving every
        // downstream reader to match on CandidateEmail. Prefers the actually-authenticated
        // submitter's own profile (covers internal-board apply, and any logged-in candidate using
        // the guest endpoint); falls back to an existing profile matching the typed email; stays
        // null for a guest with no profile yet - it gets linked later at registration (see
        // CurrentCandidateService.GetOrCreateCurrentProfileAsync's claim step).
        private async Task<long?> ResolveCandidateProfileIdAsync(string? candidateEmail)
        {
            var currentProfileId = await _currentCandidateService.TryGetCurrentCandidateProfileIdAsync();
            if (currentProfileId != null)
                return currentProfileId;

            return string.IsNullOrEmpty(candidateEmail)
                ? null
                : await _candidateProfileRepository.GetIdByEmailAsync(candidateEmail);
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

            if (source != ApplicationSourceEnum.Admin)
                await EnsureMinimumProfileCompletenessAsync(request.CandidateEmail);

            var candidateProfileId = await ResolveCandidateProfileIdAsync(request.CandidateEmail);

            // EP-17/US-127: a matching waiver rule skips payment the same way HR apply-on-behalf
            // does. Resolved before the requiresPayment gate below so it can short-circuit a
            // fee-bearing vacancy; Admin submissions never reach here, they already bypass payment.
            WaiverRule? waiverRule = null;
            if (source != ApplicationSourceEnum.Admin)
            {
                var candidateProfile = candidateProfileId.HasValue
                    ? await _candidateProfileRepository.GetByIdAsync(candidateProfileId.Value)
                    : null;
                waiverRule = await _waiverRuleService.TryMatchAsync(
                    candidateProfile?.IsInternal ?? false, request.SpecialCategoryId, request.ReferralSourceId);
            }

            // EP-17: HR applying on a candidate's behalf, or a matched fee-waiver rule, bypasses
            // payment entirely. Every other source is gated when the vacancy has a fee configured.
            var requiresPayment = source != ApplicationSourceEnum.Admin && waiverRule == null && jobPosting.ApplicationFeeAmount is > 0;

            var entity = request.ToEntity();
            entity.Source = source;
            entity.ApplicationStatus = requiresPayment ? ApplicationStatusEnum.AwaitingPayment : ApplicationStatusEnum.Applied;
            entity.AppliedDate = DateTime.UtcNow;
            entity.CandidateProfileId = candidateProfileId;
            entity.SpecialCategoryId = request.SpecialCategoryId;
            entity.ReferralSourceId = request.ReferralSourceId;

            if (waiverRule != null)
            {
                entity.WaiverRuleId = waiverRule.WaiverRuleId;
                entity.WaivedAt = DateTime.UtcNow;
            }

            if (request.Resume != null)
            {
                var (_, filePath) = await _applicationCvStorageService.SaveAsync(
                    request.Resume.OpenReadStream(), request.Resume.FileName, jobPosting.JobPostingId.ToString());
                entity.ResumeUrl = filePath;
                entity.ResumeExtractedText = await TryExtractResumeTextAsync(request.Resume);
            }
            else if (candidateProfileId.HasValue)
            {
                // No new file attached - reuse whatever resume the candidate already has on file
                // in their profile Documents (US-006), so they aren't forced to re-upload the same
                // file on every application. Copies the bytes into this application's own CV
                // storage (rather than pointing ResumeUrl at the shared document) so what HR sees
                // for this application is a stable snapshot, unaffected if the candidate later
                // replaces or deletes their profile resume.
                var existingDocuments = await _candidateDocumentRepository.GetAllByCandidateProfileIdAsync(candidateProfileId.Value);
                var existingResume = existingDocuments
                    .Where(d => d.DocumentType == CandidateDocumentTypeEnum.Resume && d.IsActive)
                    .OrderByDescending(d => d.CreatedAt)
                    .FirstOrDefault();

                if (existingResume != null)
                {
                    try
                    {
                        await using var existingStream = await _fileStorageService.OpenReadAsync(existingResume.FilePath);
                        var (_, filePath) = await _applicationCvStorageService.SaveAsync(
                            existingStream, existingResume.FileName, jobPosting.JobPostingId.ToString());
                        entity.ResumeUrl = filePath;
                        // Not re-extracted here (ExtractRawTextAsync takes an IFormFile, not a
                        // stream) - CV Bank search on this application falls back to whatever the
                        // candidate profile's own resume text already indexed, not a hard failure.
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to copy existing resume document {CandidateDocumentId} for JobPostingId {JobPostingId}; application still saved without one.", existingResume.CandidateDocumentId, jobPosting.JobPostingId);
                    }
                }
            }

            await _jobApplicationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            if (waiverRule != null)
            {
                entity.StatusHistory.Add(new ApplicationStatusHistory
                {
                    JobApplicationId = entity.JobApplicationId,
                    FromStatus = null,
                    ToStatus = entity.ApplicationStatus,
                    ChangedByUserName = WaiverSystemActor,
                    ChangedAt = DateTime.UtcNow,
                    Note = $"Application fee waived by rule '{waiverRule.Name}'"
                });
                await _unitOfWork.SaveChangesAsync();
            }

            if (source == ApplicationSourceEnum.Admin && !string.IsNullOrEmpty(request.CandidateEmail))
            {
                entity.AddDomainEvent(new JobApplicationSubmittedOnBehalfEvent
                {
                    JobApplicationId = entity.JobApplicationId,
                    CandidateEmail = request.CandidateEmail
                });
            }

            entity.JobPosting = jobPosting;

            var response = entity.ToResponse();

            if (requiresPayment)
            {
                // A gateway outage must not roll back the application that's already saved above -
                // leave PaymentRedirectUrl null so the frontend shows a manual retry path instead.
                try
                {
                    var initiateResult = await _paymentService.InitiateAsync(entity.JobApplicationId);
                    if (initiateResult.Success)
                        response.PaymentRedirectUrl = initiateResult.GatewayRedirectUrl;
                }
                catch (SslCommerzUnavailableException)
                {
                    // response.PaymentRedirectUrl stays null - frontend offers a retry button.
                }
            }

            // US-075/US-076: dispatch on submit. A payment-required submission gets no email here -
            // the candidate hasn't paid yet, so sending "action required"/status here just races the
            // SSLCommerz redirect and can arrive after they've already paid. The confirmation email
            // fires from PaymentService.HandleIpnAsync once the IPN actually confirms payment.
            if (!requiresPayment)
            {
                try
                {
                    var placeholders = BuildBaseDispatchPlaceholders(entity, jobPosting.Title);
                    await _notificationDispatchService.DispatchAsync(RecruitmentEventEnum.ApplicationSubmitted, placeholders, await BuildDispatchTargetsAsync(entity));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error dispatching submit notification for JobApplicationId {JobApplicationId}.", entity.JobApplicationId);
                }
            }

            return response;
        }

        // US-007 AC4: "A minimum completeness threshold (configurable by Admin) must be met
        // before an application can be submitted." Only fires when CandidateEmail resolves to an
        // existing CandidateProfile (there's no FK - matched by email, same precedent as
        // GetAttributeFilteredApplicationsAsync/GetMyApplicationsAsync) and the Admin has actually
        // configured a threshold (0 = disabled). A guest applicant with no profile at all - the
        // career-portal/internal-job-board flow never requires one - has nothing to measure, so is
        // let through unchanged.
        private async Task EnsureMinimumProfileCompletenessAsync(string? candidateEmail)
        {
            if (string.IsNullOrEmpty(candidateEmail))
                return;

            var threshold = await _applicationSettingService.GetMinimumProfileCompletenessPercentageAsync();
            if (threshold <= 0)
                return;

            var profiles = await _candidateProfileRepository.GetByEmailsAsync(new[] { candidateEmail });
            var profile = profiles?.FirstOrDefault(p => string.Equals(p.Email, candidateEmail, StringComparison.OrdinalIgnoreCase));
            if (profile == null)
                return;

            var completeness = CandidateProfileMapper.CalculateCompleteness(profile);
            if (completeness < threshold)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(JobApplicationSubmitRequest.CandidateEmail),
                        $"Your profile is {completeness}% complete. A minimum of {threshold}% completeness is required before you can submit an application.")
                });
            }
        }

        // Best-effort: a failed extraction must not fail the application submission itself.
        private async Task<string?> TryExtractResumeTextAsync(IFormFile resume)
        {
            var extension = Path.GetExtension(resume.FileName);
            if (!ExtractableResumeExtensions.Contains(extension))
                return null;

            try
            {
                return await _resumeParsingService.ExtractRawTextAsync(resume);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract resume text for CV Bank search; application is still saved.");
                return null;
            }
        }

        // ── ATS Dashboard / Status Update (US-035 / US-036) ───────────────

        // EP-14 US-109 AC3: these tracker columns come from a joined stage-progress row, not a
        // native JobApplication property, so PaginationExtensions' generic reflection-based
        // ApplySorting can't translate them to SQL. Sorting by one of these routes through the
        // same in-memory path already established for candidate-attribute filters below.
        private static readonly HashSet<string> StageDerivedSortKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(JobApplicationDashboardResponse.CurrentStageName),
            nameof(JobApplicationDashboardResponse.DaysInCurrentStage),
            nameof(JobApplicationDashboardResponse.IsStale),
            nameof(JobApplicationDashboardResponse.AssignedHrUserName),
            nameof(JobApplicationDashboardResponse.LastUpdatedAt)
        };

        public async Task<PagedResult<JobApplicationDashboardResponse>> GetDashboardPagedAsync(
            PagedRequest request,
            JobApplicationAttributeFilterRequest filter)
        {
            ValidateAttributeFilterRequest(filter);

            var needsInMemoryPath = filter.HasCandidateAttributeFilters
                || filter.StaleOnly == true
                || (!string.IsNullOrEmpty(request.SortBy) && StageDerivedSortKeys.Contains(request.SortBy));

            if (!needsInMemoryPath)
            {
                var pagedResult = await _jobApplicationRepository.GetPaginatedAllAsync(
                    request, filter.JobPostingId, filter.Status, filter.Source, filter.DateFrom, filter.DateTo);

                var pageResponses = pagedResult.Data.Select(e => e.ToDashboardResponse()).ToList();
                await AttachStageProgressInfoAsync(pageResponses);

                return new PagedResult<JobApplicationDashboardResponse>
                {
                    Data = pageResponses,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }

            // Candidate-attribute filters, StaleOnly, and stage-derived sort keys all require an
            // in-memory pass (email->profile join has no FK; staleness/stage aren't native
            // JobApplication columns) - pagination happens after filtering/sorting here rather
            // than in SQL, same tradeoff already accepted by the candidate-attribute path.
            var matched = filter.HasCandidateAttributeFilters
                ? await GetAttributeFilteredApplicationsAsync(filter)
                : await _jobApplicationRepository.GetAllMatchingAsync(filter.JobPostingId, filter.Status, filter.Source, filter.DateFrom, filter.DateTo);

            var allResponses = matched.Select(e => e.ToDashboardResponse()).ToList();
            await AttachStageProgressInfoAsync(allResponses);

            if (filter.StaleOnly == true)
                allResponses = allResponses.Where(r => r.IsStale).ToList();

            allResponses = ApplyStageDerivedSort(allResponses, request.SortBy, request.SortDirection);

            var page = allResponses
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResult<JobApplicationDashboardResponse>
            {
                Data = page,
                TotalCount = allResponses.Count,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }

        /// <summary>Batch-fills the stage-derived tracker columns (Stage/DaysInCurrentStage/
        /// IsStale/AssignedHR/LastUpdated) on an already-mapped response page, one query for the
        /// whole batch rather than per-row.</summary>
        private async Task AttachStageProgressInfoAsync(List<JobApplicationDashboardResponse> responses)
        {
            if (responses.Count == 0)
                return;

            var ids = responses.Select(r => r.JobApplicationId).ToList();
            var currentByAppId = await _jobApplicationStageProgressRepository.GetCurrentByJobApplicationIdsAsync(ids);
            var defaultStaleDaysThreshold = await _applicationSettingService.GetDefaultStaleDaysThresholdAsync();
            var now = DateTime.UtcNow;

            foreach (var response in responses)
            {
                if (!currentByAppId.TryGetValue(response.JobApplicationId, out var current))
                    continue;

                response.CurrentStageName = current.StageName;
                response.AssignedHrUserName = current.LastUpdatedByUserName;
                // Audit.UpdatedAt is never actually stamped anywhere in this codebase (confirmed -
                // no interceptor, no manual stamp on this entity), so it's always null and useless
                // here. StageEnteredAt is the one real timestamp this row carries.
                response.LastUpdatedAt = current.StageEnteredAt;

                if (current.StageEnteredAt.HasValue)
                {
                    var daysInStage = (int)(now - current.StageEnteredAt.Value).TotalDays;
                    response.DaysInCurrentStage = daysInStage;

                    var threshold = current.SlaDaysSnapshot ?? defaultStaleDaysThreshold;
                    response.IsStale = threshold.HasValue && daysInStage > threshold.Value;
                }
            }
        }

        private static List<JobApplicationDashboardResponse> ApplyStageDerivedSort(
            List<JobApplicationDashboardResponse> responses, string? sortBy, string? sortDirection)
        {
            var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(sortBy) || !StageDerivedSortKeys.Contains(sortBy))
                return responses.OrderByDescending(r => r.AppliedDate).ToList();

            IOrderedEnumerable<JobApplicationDashboardResponse> ordered = sortBy.ToLowerInvariant() switch
            {
                "currentstagename" => descending
                    ? responses.OrderByDescending(r => r.CurrentStageName)
                    : responses.OrderBy(r => r.CurrentStageName),
                "daysincurrentstage" => descending
                    ? responses.OrderByDescending(r => r.DaysInCurrentStage)
                    : responses.OrderBy(r => r.DaysInCurrentStage),
                "isstale" => descending
                    ? responses.OrderByDescending(r => r.IsStale)
                    : responses.OrderBy(r => r.IsStale),
                "assignedhrusername" => descending
                    ? responses.OrderByDescending(r => r.AssignedHrUserName)
                    : responses.OrderBy(r => r.AssignedHrUserName),
                "lastupdatedat" => descending
                    ? responses.OrderByDescending(r => r.LastUpdatedAt)
                    : responses.OrderBy(r => r.LastUpdatedAt),
                _ => responses.OrderByDescending(r => r.AppliedDate)
            };

            return ordered.ToList();
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

            if (filter.StaleOnly == true)
            {
                var matchedForStale = filter.HasCandidateAttributeFilters
                    ? await GetAttributeFilteredApplicationsAsync(filter)
                    : await _jobApplicationRepository.GetAllMatchingAsync(filter.JobPostingId, filter.Status, filter.Source, filter.DateFrom, filter.DateTo);

                var responses = matchedForStale.Select(e => e.ToDashboardResponse()).ToList();
                await AttachStageProgressInfoAsync(responses);
                return responses.Where(r => r.IsStale).Select(r => r.JobApplicationId).ToList();
            }

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
                    var placeholders = BuildBaseDispatchPlaceholders(entity, jobPosting?.Title ?? string.Empty);

                    await _notificationDispatchService.DispatchAsync(
                        request.RecruitmentEvent,
                        placeholders,
                        await BuildDispatchTargetsAsync(entity),
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

        // US-101: batches above this size go through the EP-13 F1 async export queue
        // (IExportRequestService.RequestBulkCvZipExportAsync) instead - QuestPDF rendering per
        // candidate is heavier than the cheap row-append bulk-status/bulk-notify do, so the
        // sync-download cap is intentionally much lower than BulkNotifyAsync's 500.
        public const int BulkDownloadCvsSyncMaxCount = 20;

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

            // US-075: covers both UpdateStatusAsync (single) and BulkUpdateStatusAsync (loop) -
            // both funnel through here. persistImmediately:false so the NotificationLog row rides
            // along in the caller's own SaveChangesAsync instead of committing early.
            try
            {
                var jobPosting = await _jobPostingRepository.GetByIdAsync(entity.JobPostingId);
                var placeholders = BuildBaseDispatchPlaceholders(entity, jobPosting?.Title ?? string.Empty);
                placeholders["FromStatus"] = fromStatus.ToString();
                placeholders["ToStatus"] = request.ToStatus.ToString();

                await _notificationDispatchService.DispatchAsync(
                    RecruitmentEventEnum.ApplicationStatusChanged,
                    placeholders,
                    await BuildDispatchTargetsAsync(entity),
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
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var email = await _currentCandidateService.GetCurrentEmailAsync();
            var applications = await _jobApplicationRepository.GetByCandidateAsync(profileId, email);

            return applications
                .Select(a => a.ToMyApplicationResponse(CanWithdraw(a.ApplicationStatus, a.JobPosting?.Status)))
                .ToList();
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

            EnsureLegalStatusTransition(entity.ApplicationStatus, ApplicationStatusEnum.Withdrawn);

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

            // US-075: separate hook from ApplyStatusChangeAsync since this path duplicates the
            // status-history logic inline rather than calling it.
            try
            {
                var placeholders = BuildBaseDispatchPlaceholders(entity, jobPosting?.Title ?? string.Empty);
                placeholders["FromStatus"] = fromStatus.ToString();
                placeholders["ToStatus"] = ApplicationStatusEnum.Withdrawn.ToString();

                await _notificationDispatchService.DispatchAsync(
                    RecruitmentEventEnum.ApplicationWithdrawn,
                    placeholders,
                    await BuildDispatchTargetsAsync(entity),
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
                && LegalStatusTransitions.TryGetValue(currentStatus, out var allowedTransitions)
                && allowedTransitions.Contains(ApplicationStatusEnum.Withdrawn);
        }

        public async Task<JobEligibilityResponse> CheckEligibilityAsync(long jobPostingId)
        {
            var jobPosting = await _jobPostingRepository.GetByIdAsync(jobPostingId)
                ?? throw new NotFoundException("JobPosting", jobPostingId);

            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var profile = await _candidateProfileRepository.GetByIdWithIncludeAsync(
                p => p.CandidateProfileId == profileId,
                p => p.Educations, p => p.WorkExperiences, p => p.Skills);

            var facts = CandidateFactService.BuildFacts(profile);
            var unmetRequirements = JobEligibilityEvaluator.Evaluate(jobPosting, facts);

            return new JobEligibilityResponse
            {
                IsEligible = unmetRequirements.Count == 0,
                UnmetRequirements = unmetRequirements
            };
        }

        // ── Duplicate Detection (US-038) ──────────────────────────────────

        public async Task<List<JobApplicationDuplicateGroupResponse>> GetDuplicatesAsync(long jobPostingId)
        {
            var applications = await _jobApplicationRepository.GetAllByJobPostingIdAsync(jobPostingId);
            return BuildDuplicateGroups(applications);
        }

        public async Task ResolveDuplicatesAsync(JobApplicationDuplicateResolveRequest request)
        {
            if (request.DuplicateJobApplicationIds.Count == 0)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DuplicateJobApplicationIds),
                        "At least one duplicate application id is required.")
                });
            }

            if (request.DuplicateJobApplicationIds.Contains(request.PrimaryJobApplicationId))
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.PrimaryJobApplicationId),
                        "PrimaryJobApplicationId must not also appear in DuplicateJobApplicationIds.")
                });
            }

            // Re-derive the duplicate group server-side rather than trusting the client's grouping,
            // so HR can't dismiss an application that isn't actually part of a detected duplicate set.
            var groups = await GetDuplicatesAsync(request.JobPostingId);
            var group = groups.FirstOrDefault(g => g.Applications.Any(a => a.JobApplicationId == request.PrimaryJobApplicationId));

            if (group == null || request.DuplicateJobApplicationIds.Any(id => !group.Applications.Any(a => a.JobApplicationId == id)))
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DuplicateJobApplicationIds),
                        "The supplied applications are not a detected duplicate group for this vacancy.")
                });
            }

            foreach (var duplicateId in request.DuplicateJobApplicationIds)
            {
                var entity = await _jobApplicationRepository.GetByIdAsync(duplicateId)
                    ?? throw new NotFoundException("JobApplication", duplicateId);

                await ApplyStatusChangeAsync(entity, new JobApplicationStatusUpdateRequest
                {
                    ToStatus = ApplicationStatusEnum.DuplicateDismissed,
                    Note = $"Duplicate of application #{request.PrimaryJobApplicationId}, dismissed by HR."
                });
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private static List<JobApplicationDuplicateGroupResponse> BuildDuplicateGroups(List<JobApplication> applications)
        {
            var candidates = applications
                .Where(a => a.ApplicationStatus != ApplicationStatusEnum.DuplicateDismissed)
                .ToList();

            var parent = Enumerable.Range(0, candidates.Count).ToArray();

            int Find(int i) => parent[i] == i ? i : (parent[i] = Find(parent[i]));
            void Union(int a, int b)
            {
                var rootA = Find(a);
                var rootB = Find(b);
                if (rootA != rootB) parent[rootA] = rootB;
            }

            for (var i = 0; i < candidates.Count; i++)
            {
                for (var j = i + 1; j < candidates.Count; j++)
                {
                    if (AreDuplicates(candidates[i], candidates[j]))
                        Union(i, j);
                }
            }

            return candidates
                .Select((app, index) => (app, root: Find(index)))
                .GroupBy(x => x.root)
                .Where(g => g.Count() > 1)
                .Select(g =>
                {
                    var members = g.Select(x => x.app).ToList();
                    return new JobApplicationDuplicateGroupResponse
                    {
                        Applications = members.OrderBy(m => m.AppliedDate).Select(m => m.ToDuplicateItemResponse()).ToList(),
                        MatchedOn = DetermineMatchedOn(members)
                    };
                })
                .ToList();
        }

        private static bool AreDuplicates(JobApplication a, JobApplication b)
        {
            if (!string.IsNullOrEmpty(a.CandidateEmail) && !string.IsNullOrEmpty(b.CandidateEmail)
                && string.Equals(a.CandidateEmail, b.CandidateEmail, StringComparison.OrdinalIgnoreCase))
                return true;

            var phoneA = PhoneNormalizer.Normalize(a.CandidatePhone);
            var phoneB = PhoneNormalizer.Normalize(b.CandidatePhone);
            if (phoneA != null && phoneB != null && phoneA == phoneB)
                return true;

            if (!string.IsNullOrEmpty(a.CandidateNationalId) && !string.IsNullOrEmpty(b.CandidateNationalId)
                && string.Equals(a.CandidateNationalId, b.CandidateNationalId, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private static List<string> DetermineMatchedOn(List<JobApplication> members)
        {
            var pairs = members
                .SelectMany((a, i) => members.Skip(i + 1).Select(b => (a, b)))
                .ToList();

            var reasons = new List<string>();

            if (pairs.Any(p => !string.IsNullOrEmpty(p.a.CandidateEmail) && !string.IsNullOrEmpty(p.b.CandidateEmail)
                && string.Equals(p.a.CandidateEmail, p.b.CandidateEmail, StringComparison.OrdinalIgnoreCase)))
                reasons.Add("Email");

            if (pairs.Any(p =>
            {
                var phoneA = PhoneNormalizer.Normalize(p.a.CandidatePhone);
                var phoneB = PhoneNormalizer.Normalize(p.b.CandidatePhone);
                return phoneA != null && phoneB != null && phoneA == phoneB;
            }))
                reasons.Add("Phone");

            if (pairs.Any(p => !string.IsNullOrEmpty(p.a.CandidateNationalId) && !string.IsNullOrEmpty(p.b.CandidateNationalId)
                && string.Equals(p.a.CandidateNationalId, p.b.CandidateNationalId, StringComparison.OrdinalIgnoreCase)))
                reasons.Add("NationalId");

            return reasons;
        }
    }
}
