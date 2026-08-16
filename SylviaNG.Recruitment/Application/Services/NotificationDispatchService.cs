using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Common.Notifications;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class NotificationDispatchService : INotificationDispatchService
    {
        private readonly IEventTemplateMappingRepository _eventTemplateMappingRepository;
        private readonly INotificationLogRepository _notificationLogRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IPlaceholderSubstitutionService _placeholderSubstitutionService;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationDispatchService> _logger;

        public NotificationDispatchService(
            IEventTemplateMappingRepository eventTemplateMappingRepository,
            INotificationLogRepository notificationLogRepository,
            IUserAccountRepository userAccountRepository,
            IJobApplicationRepository jobApplicationRepository,
            IPlaceholderSubstitutionService placeholderSubstitutionService,
            ISmtpEmailService smtpEmailService,
            IUnitOfWork unitOfWork,
            ILogger<NotificationDispatchService> logger)
        {
            _eventTemplateMappingRepository = eventTemplateMappingRepository;
            _notificationLogRepository = notificationLogRepository;
            _userAccountRepository = userAccountRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _placeholderSubstitutionService = placeholderSubstitutionService;
            _smtpEmailService = smtpEmailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<NotificationDispatchResult> DispatchAsync(
            RecruitmentEventEnum recruitmentEvent,
            IDictionary<string, string> placeholderValues,
            NotificationDispatchTargets targets,
            bool persistImmediately = true,
            IReadOnlyList<EmailAttachment>? attachments = null,
            CancellationToken cancellationToken = default)
        {
            EmailSendResult? candidateResult = null;
            EmailSendResult? adminHrResult = null;

            if (!string.IsNullOrWhiteSpace(targets.CandidateEmail))
                candidateResult = await DispatchToRecipientAsync(recruitmentEvent, NotificationRecipientTypeEnum.Candidate, targets.CandidateEmail!, placeholderValues, targets.JobApplicationId, attachments, cancellationToken);

            var adminHrRecipients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(targets.AdminHrEmail))
                adminHrRecipients.Add(targets.AdminHrEmail);

            if (targets.NotifyActiveHrUsers)
            {
                var activeHrEmails = await _userAccountRepository.GetActiveEmailsByRoleAsync("HR");
                adminHrRecipients.UnionWith(activeHrEmails);
            }

            foreach (var recipient in adminHrRecipients)
            {
                var result = await DispatchToRecipientAsync(recruitmentEvent, NotificationRecipientTypeEnum.AdminHr, recipient, placeholderValues, targets.JobApplicationId, attachments, cancellationToken);
                adminHrResult ??= result;
                if (!result.Success)
                    adminHrResult = result;
            }

            if (persistImmediately)
            {
                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to persist NotificationLog rows for {RecruitmentEvent}.", recruitmentEvent);
                }
            }

            return new NotificationDispatchResult(candidateResult, adminHrResult);
        }

        private async Task<EmailSendResult> DispatchToRecipientAsync(
            RecruitmentEventEnum recruitmentEvent,
            NotificationRecipientTypeEnum recipientType,
            string address,
            IDictionary<string, string> placeholderValues,
            long? jobApplicationId,
            IReadOnlyList<EmailAttachment>? attachments,
            CancellationToken cancellationToken)
        {
            // Multi-tenant: mirrors the triggering JobApplication's CompanyId, so the bell/log
            // feed stays scoped to one company (JobApplication is already ICompanyScoped).
            // Events with no JobApplicationId (e.g. candidate account/OTP mail) leave this null -
            // acceptable since those never surface in the HR bell to begin with.
            long? companyId = null;
            if (jobApplicationId.HasValue)
            {
                var jobApplication = await _jobApplicationRepository.GetByIdAsync(jobApplicationId.Value);
                companyId = jobApplication?.CompanyId;
            }

            var log = new NotificationLog
            {
                RecruitmentEvent = recruitmentEvent,
                Channel = NotificationChannelEnum.Email,
                RecipientType = recipientType,
                RecipientAddress = address,
                JobApplicationId = jobApplicationId,
                CompanyId = companyId,
                // Audit.CreatedAt isn't auto-stamped anywhere (no SaveChanges interceptor for it,
                // unlike UtcDateTimeInterceptor's UTC-normalization) - every repo query that orders
                // the bell/log list by CreatedAt (NotificationLogRepository.GetUnreadFor*Async,
                // GetFilteredQueryable) silently no-ops without this, since a column that's always
                // null sorts as "no order" and Postgres falls back to insertion order.
                CreatedAt = DateTime.UtcNow,
            };

            var sendResult = new EmailSendResult { Success = false, ErrorMessage = "No active template mapping" };

            try
            {
                var mapping = await _eventTemplateMappingRepository.GetActiveMappingAsync(recruitmentEvent, NotificationChannelEnum.Email, recipientType);
                if (mapping == null)
                {
                    log.DeliveryStatus = NotificationStatusEnum.Skipped;
                    log.FailureReason = "No active template mapping";
                }
                else
                {
                    log.NotificationTemplateId = mapping.NotificationTemplateId;

                    var renderedSubject = _placeholderSubstitutionService.Render(mapping.NotificationTemplate.Subject ?? string.Empty, placeholderValues);
                    var renderedBody = _placeholderSubstitutionService.Render(mapping.NotificationTemplate.Body, placeholderValues);
                    log.RenderedSubject = renderedSubject;
                    log.RenderedBody = renderedBody;

                    sendResult = await EmailRetrySender.SendWithRetryAsync(_smtpEmailService, new EmailMessage
                    {
                        To = address,
                        Subject = renderedSubject,
                        HtmlBody = renderedBody,
                        Attachments = attachments?.ToList() ?? new List<EmailAttachment>()
                    }, _logger, cancellationToken: cancellationToken);

                    if (sendResult.Success)
                    {
                        log.DeliveryStatus = NotificationStatusEnum.Sent;
                        log.SentAt = DateTime.UtcNow;
                    }
                    else
                    {
                        log.DeliveryStatus = NotificationStatusEnum.Failed;
                        log.FailureReason = sendResult.ErrorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error dispatching {RecruitmentEvent}/{RecipientType} to {Address}.", recruitmentEvent, recipientType, address);
                log.DeliveryStatus = NotificationStatusEnum.Failed;
                log.FailureReason = ex.Message;
                sendResult = new EmailSendResult { Success = false, ErrorMessage = ex.Message };
            }

            await _notificationLogRepository.AddAsync(log);
            return sendResult;
        }
    }
}
