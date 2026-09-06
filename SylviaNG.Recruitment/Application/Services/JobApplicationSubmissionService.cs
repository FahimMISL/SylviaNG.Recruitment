using Microsoft.AspNetCore.Http;
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
    public class JobApplicationSubmissionService : IJobApplicationSubmissionService
    {
        private static readonly HashSet<string> ExtractableResumeExtensions = new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".docx" };

        private static readonly CircularTypeEnum[] ExternalCircularTypes = { CircularTypeEnum.ExternalOnly, CircularTypeEnum.Both };
        private static readonly CircularTypeEnum[] InternalCircularTypes = { CircularTypeEnum.InternalOnly, CircularTypeEnum.Both };

        // US-034 AC1: HR applying on a candidate's behalf can target any open vacancy, regardless
        // of its audience restriction.
        private static readonly CircularTypeEnum[] AdminCircularTypes = { CircularTypeEnum.ExternalOnly, CircularTypeEnum.InternalOnly, CircularTypeEnum.Both };

        private const string WaiverSystemActor = "system:fee-waiver";

        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly ICandidateDocumentRepository _candidateDocumentRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IApplicationCvStorageService _applicationCvStorageService;
        private readonly IWaiverRuleService _waiverRuleService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IResumeParsingService _resumeParsingService;
        private readonly IPaymentService _paymentService;
        private readonly INotificationDispatchQueue _notificationDispatchQueue;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<JobApplicationSubmissionService> _logger;
        private readonly ICandidateApplicationLinkResolver _candidateApplicationLinkResolver;
        private readonly IJobApplicationDispatchTargetBuilder _dispatchTargetBuilder;

        public JobApplicationSubmissionService(
            IJobApplicationRepository jobApplicationRepository,
            IJobPostingRepository jobPostingRepository,
            ICandidateProfileRepository candidateProfileRepository,
            ICandidateDocumentRepository candidateDocumentRepository,
            IFileStorageService fileStorageService,
            IApplicationCvStorageService applicationCvStorageService,
            IWaiverRuleService waiverRuleService,
            IApplicationSettingService applicationSettingService,
            IResumeParsingService resumeParsingService,
            IPaymentService paymentService,
            INotificationDispatchQueue notificationDispatchQueue,
            IUnitOfWork unitOfWork,
            ILogger<JobApplicationSubmissionService> logger,
            ICandidateApplicationLinkResolver candidateApplicationLinkResolver,
            IJobApplicationDispatchTargetBuilder dispatchTargetBuilder)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _jobPostingRepository = jobPostingRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _candidateDocumentRepository = candidateDocumentRepository;
            _fileStorageService = fileStorageService;
            _applicationCvStorageService = applicationCvStorageService;
            _waiverRuleService = waiverRuleService;
            _applicationSettingService = applicationSettingService;
            _resumeParsingService = resumeParsingService;
            _paymentService = paymentService;
            _notificationDispatchQueue = notificationDispatchQueue;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _candidateApplicationLinkResolver = candidateApplicationLinkResolver;
            _dispatchTargetBuilder = dispatchTargetBuilder;
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

            var candidateProfileId = await _candidateApplicationLinkResolver.ResolveCandidateProfileIdAsync(request.CandidateEmail);

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

                // A rule matching the claimed category isn't enough - the claim must carry
                // proof, otherwise anyone could pick "Freedom Fighter" off the dropdown and
                // skip the fee unverified. No document = no waiver, applicant still pays.
                if (waiverRule != null && request.WaiverProofDocument == null)
                    waiverRule = null;
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

                // Only stored once the rule actually matched - avoids keeping an upload for a
                // category/rule combination that never resulted in a waiver.
                var (_, waiverProofPath) = await _applicationCvStorageService.SaveAsync(
                    request.WaiverProofDocument!.OpenReadStream(), request.WaiverProofDocument!.FileName, jobPosting.JobPostingId.ToString());
                entity.WaiverProofDocumentUrl = waiverProofPath;
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
                    var initiateResult = await _paymentService.InitiateAsync(entity.JobApplicationId, entity.CandidateEmail ?? string.Empty);
                    if (initiateResult.Success)
                        response.PaymentRedirectUrl = initiateResult.GatewayRedirectUrl;
                }
                catch (SslCommerzUnavailableException)
                {
                    // response.PaymentRedirectUrl stays null - frontend offers a retry button.
                }
            }

            // Queued, not awaited: this response carries PaymentRedirectUrl, and the applicant's
            // browser can't leave for the gateway until it arrives. Awaiting the dispatch put a
            // full SMTP session per recipient - candidate plus every active HR mailbox, each
            // with EmailRetrySender's 3 bounded attempts - between clicking Submit and the
            // checkout page opening, on top of the CV upload, the PDF text extraction and the
            // SSLCommerz session call this method already does inline.
            var placeholders = JobApplicationDispatchTargetBuilder.BuildBasePlaceholders(entity, jobPosting.Title);
            var targets = await _dispatchTargetBuilder.BuildTargetsAsync(entity);

            if (requiresPayment)
            {
                placeholders["FeeAmount"] = jobPosting.ApplicationFeeAmount?.ToString("N2") ?? string.Empty;
                placeholders["PaymentLink"] = response.PaymentRedirectUrl ?? string.Empty;
                _notificationDispatchQueue.TryEnqueue(new NotificationDispatchRequest(RecruitmentEventEnum.CandidateActionRequired, placeholders, targets));
            }
            else
            {
                _notificationDispatchQueue.TryEnqueue(new NotificationDispatchRequest(RecruitmentEventEnum.ApplicationSubmitted, placeholders, targets));
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
    }
}
