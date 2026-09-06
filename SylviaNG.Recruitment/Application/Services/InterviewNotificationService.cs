using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Composes and sends the schedule/reschedule/cancel email+SMS for a single Interview
    /// (EP-08). Deliberately never throws - every risky section is wrapped so a mail-server or
    /// SMS-gateway failure never blocks the caller's action; the outcome is written onto the
    /// interview row instead. Email routes through INotificationDispatchService (one
    /// RecruitmentEventEnum per scheduled/rescheduled/cancelled) so its copy lives in the same
    /// DB-editable template system as every other notification, not a hardcoded C# string. SMS has
    /// no equivalent templating system anywhere in this codebase, so it stays a plain string.
    /// </summary>
    public class InterviewNotificationService : IInterviewNotificationService
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly ISmsNotificationService _smsNotificationService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InterviewNotificationService> _logger;

        public InterviewNotificationService(
            IInterviewRepository interviewRepository,
            INotificationDispatchService notificationDispatchService,
            ISmsNotificationService smsNotificationService,
            IApplicationSettingService applicationSettingService,
            IUnitOfWork unitOfWork,
            ILogger<InterviewNotificationService> logger)
        {
            _interviewRepository = interviewRepository;
            _notificationDispatchService = notificationDispatchService;
            _smsNotificationService = smsNotificationService;
            _applicationSettingService = applicationSettingService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public Task NotifyScheduledAsync(Interview interview) =>
            NotifyAsync(interview, RecruitmentEventEnum.InterviewScheduled, BuildScheduledSms);

        public Task NotifyRescheduledAsync(Interview interview) =>
            NotifyAsync(interview, RecruitmentEventEnum.InterviewRescheduled, BuildRescheduledSms);

        public Task NotifyCancelledAsync(Interview interview) =>
            NotifyAsync(interview, RecruitmentEventEnum.InterviewCancelled, BuildCancelledSms);

        private async Task NotifyAsync(
            Interview interview,
            RecruitmentEventEnum recruitmentEvent,
            Func<Interview, string> buildSms)
        {
            try
            {
                await SendEmailAsync(interview, recruitmentEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending interview email for InterviewId {InterviewId}.", interview.InterviewId);
                interview.EmailNotificationStatus = NotificationStatusEnum.Failed;
                interview.EmailFailureReason = ex.Message;
            }

            try
            {
                await SendSmsAsync(interview, buildSms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending interview SMS for InterviewId {InterviewId}.", interview.InterviewId);
                interview.SmsNotificationStatus = NotificationStatusEnum.Failed;
            }

            try
            {
                _interviewRepository.Update(interview);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist notification status for InterviewId {InterviewId}.", interview.InterviewId);
            }
        }

        private async Task SendEmailAsync(Interview interview, RecruitmentEventEnum recruitmentEvent)
        {
            var candidateEmail = interview.JobApplication?.CandidateEmail;
            if (string.IsNullOrWhiteSpace(candidateEmail))
            {
                interview.EmailNotificationStatus = NotificationStatusEnum.Skipped;
                return;
            }

            var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = interview.JobApplication?.CandidateName ?? string.Empty,
                // Both are stored UTC (timestamptz) - every other display path (API JSON
                // responses) converts back to local via LocalDateTimeJsonConverter; this
                // hand-built email string bypassed that entirely and showed raw UTC.
                ["ScheduledStartAt"] = DateTimeUtility.ConvertUtcToLocal(interview.ScheduledStartAt).ToString("dddd, dd MMM yyyy hh:mm tt"),
                ["ScheduledEndAt"] = DateTimeUtility.ConvertUtcToLocal(interview.ScheduledEndAt).ToString("hh:mm tt"),
                ["LocationLabel"] = interview.InterviewType == InterviewTypeEnum.Virtual ? "Meeting Link" : "Venue",
                ["LocationValue"] = interview.InterviewType == InterviewTypeEnum.Virtual
                    ? interview.MeetingLink ?? string.Empty
                    : $"{interview.InterviewVenue?.VenueName} - Room: {interview.InterviewRoom?.RoomName}",
                ["CancellationReason"] = interview.CancellationReason ?? string.Empty
            };

            var dispatchResult = await _notificationDispatchService.DispatchAsync(
                recruitmentEvent,
                placeholders,
                new NotificationDispatchTargets(candidateEmail, await _applicationSettingService.GetHrNotificationEmailAsync(), interview.JobApplicationId, NotifyActiveHrUsers: true));

            if (dispatchResult.CandidateResult?.Success == true)
            {
                interview.EmailNotificationStatus = NotificationStatusEnum.Sent;
                interview.EmailSentAt = DateTime.UtcNow;
                interview.EmailFailureReason = null;
            }
            else
            {
                interview.EmailNotificationStatus = NotificationStatusEnum.Failed;
                interview.EmailFailureReason = dispatchResult.CandidateResult?.ErrorMessage ?? "Unknown error";
            }
        }

        private async Task SendSmsAsync(Interview interview, Func<Interview, string> buildSms)
        {
            var candidatePhone = interview.JobApplication?.CandidatePhone;
            if (string.IsNullOrWhiteSpace(candidatePhone))
            {
                interview.SmsNotificationStatus = NotificationStatusEnum.Skipped;
                return;
            }

            var sent = await _smsNotificationService.TrySendAsync(candidatePhone, buildSms(interview));

            if (sent)
            {
                interview.SmsNotificationStatus = NotificationStatusEnum.Sent;
                interview.SmsLoggedAt = DateTime.UtcNow;
            }
            else
            {
                interview.SmsNotificationStatus = NotificationStatusEnum.Failed;
            }
        }

        private static string BuildScheduledSms(Interview interview) =>
            $"Your interview is scheduled on {DateTimeUtility.ConvertUtcToLocal(interview.ScheduledStartAt):dd MMM yyyy hh:mm tt}.";

        private static string BuildRescheduledSms(Interview interview) =>
            $"Your interview has been rescheduled to {DateTimeUtility.ConvertUtcToLocal(interview.ScheduledStartAt):dd MMM yyyy hh:mm tt}.";

        private static string BuildCancelledSms(Interview interview) =>
            $"Your interview scheduled on {DateTimeUtility.ConvertUtcToLocal(interview.ScheduledStartAt):dd MMM yyyy hh:mm tt} has been cancelled.";
    }
}
