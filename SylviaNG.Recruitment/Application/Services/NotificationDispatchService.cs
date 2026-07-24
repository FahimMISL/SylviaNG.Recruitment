using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Common.Email;
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
        private readonly IPlaceholderSubstitutionService _placeholderSubstitutionService;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationDispatchService> _logger;

        public NotificationDispatchService(
            IEventTemplateMappingRepository eventTemplateMappingRepository,
            INotificationLogRepository notificationLogRepository,
            IPlaceholderSubstitutionService placeholderSubstitutionService,
            ISmtpEmailService smtpEmailService,
            IUnitOfWork unitOfWork,
            ILogger<NotificationDispatchService> logger)
        {
            _eventTemplateMappingRepository = eventTemplateMappingRepository;
            _notificationLogRepository = notificationLogRepository;
            _placeholderSubstitutionService = placeholderSubstitutionService;
            _smtpEmailService = smtpEmailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task DispatchAsync(
            RecruitmentEventEnum recruitmentEvent,
            IDictionary<string, string> placeholderValues,
            NotificationDispatchTargets targets,
            bool persistImmediately = true,
            CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(targets.CandidateEmail))
                await DispatchToRecipientAsync(recruitmentEvent, NotificationRecipientTypeEnum.Candidate, targets.CandidateEmail!, placeholderValues, targets.JobApplicationId, cancellationToken);

            if (!string.IsNullOrWhiteSpace(targets.AdminHrEmail))
                await DispatchToRecipientAsync(recruitmentEvent, NotificationRecipientTypeEnum.AdminHr, targets.AdminHrEmail!, placeholderValues, targets.JobApplicationId, cancellationToken);

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
        }

        private async Task DispatchToRecipientAsync(
            RecruitmentEventEnum recruitmentEvent,
            NotificationRecipientTypeEnum recipientType,
            string address,
            IDictionary<string, string> placeholderValues,
            long? jobApplicationId,
            CancellationToken cancellationToken)
        {
            var log = new NotificationLog
            {
                RecruitmentEvent = recruitmentEvent,
                Channel = NotificationChannelEnum.Email,
                RecipientType = recipientType,
                RecipientAddress = address,
                JobApplicationId = jobApplicationId
            };

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

                    var result = await _smtpEmailService.TrySendAsync(new EmailMessage
                    {
                        To = address,
                        Subject = renderedSubject,
                        HtmlBody = renderedBody
                    }, cancellationToken);

                    if (result.Success)
                    {
                        log.DeliveryStatus = NotificationStatusEnum.Sent;
                        log.SentAt = DateTime.UtcNow;
                    }
                    else
                    {
                        log.DeliveryStatus = NotificationStatusEnum.Failed;
                        log.FailureReason = result.ErrorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error dispatching {RecruitmentEvent}/{RecipientType} to {Address}.", recruitmentEvent, recipientType, address);
                log.DeliveryStatus = NotificationStatusEnum.Failed;
                log.FailureReason = ex.Message;
            }

            await _notificationLogRepository.AddAsync(log);
        }
    }
}
