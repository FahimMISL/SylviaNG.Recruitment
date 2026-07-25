using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Composes and sends the schedule/reschedule/cancel email+SMS for a single Interview
    /// (EP-08). Deliberately never throws - every risky section is wrapped so a mail-server or
    /// SMS-gateway failure never blocks the caller's action; the outcome is written onto the
    /// interview row instead. Same shape as ExamNotificationService.
    /// </summary>
    public class InterviewNotificationService : IInterviewNotificationService
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly ISmsNotificationService _smsNotificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InterviewNotificationService> _logger;

        public InterviewNotificationService(
            IInterviewRepository interviewRepository,
            ISmtpEmailService smtpEmailService,
            ISmsNotificationService smsNotificationService,
            IUnitOfWork unitOfWork,
            ILogger<InterviewNotificationService> logger)
        {
            _interviewRepository = interviewRepository;
            _smtpEmailService = smtpEmailService;
            _smsNotificationService = smsNotificationService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public Task NotifyScheduledAsync(Interview interview) =>
            NotifyAsync(interview, "Interview Scheduled", BuildScheduledBody, BuildScheduledSms);

        public Task NotifyRescheduledAsync(Interview interview) =>
            NotifyAsync(interview, "Interview Rescheduled", BuildRescheduledBody, BuildRescheduledSms);

        public Task NotifyCancelledAsync(Interview interview) =>
            NotifyAsync(interview, "Interview Cancelled", BuildCancelledBody, BuildCancelledSms);

        private async Task NotifyAsync(
            Interview interview,
            string subjectPrefix,
            Func<Interview, string> buildBody,
            Func<Interview, string> buildSms)
        {
            try
            {
                await SendEmailAsync(interview, subjectPrefix, buildBody);
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

        private async Task SendEmailAsync(Interview interview, string subjectPrefix, Func<Interview, string> buildBody)
        {
            var candidateEmail = interview.JobApplication?.CandidateEmail;
            if (string.IsNullOrWhiteSpace(candidateEmail))
            {
                interview.EmailNotificationStatus = NotificationStatusEnum.Skipped;
                return;
            }

            var message = new EmailMessage
            {
                To = candidateEmail,
                Subject = $"{subjectPrefix} - {interview.JobApplication?.CandidateName}",
                HtmlBody = buildBody(interview),
            };

            var result = await _smtpEmailService.TrySendAsync(message);

            if (result.Success)
            {
                interview.EmailNotificationStatus = NotificationStatusEnum.Sent;
                interview.EmailSentAt = DateTime.UtcNow;
                interview.EmailFailureReason = null;
            }
            else
            {
                interview.EmailNotificationStatus = NotificationStatusEnum.Failed;
                interview.EmailFailureReason = result.ErrorMessage;
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

        private static string LocationLine(Interview interview) =>
            interview.InterviewType == InterviewTypeEnum.Virtual
                ? $"<p><strong>Meeting Link:</strong> {interview.MeetingLink}</p>"
                : $"<p><strong>Venue:</strong> {interview.InterviewVenue?.VenueName} - Room: {interview.InterviewRoom?.RoomName}</p>";

        private static string BuildScheduledBody(Interview interview) => $@"
<p>Dear {interview.JobApplication?.CandidateName},</p>
<p>Your interview has been scheduled:</p>
<p><strong>Date/Time:</strong> {interview.ScheduledStartAt:dddd, dd MMM yyyy HH:mm} - {interview.ScheduledEndAt:HH:mm}</p>
{LocationLine(interview)}
<p>Regards,<br/>SylviaNG Recruitment</p>";

        private static string BuildRescheduledBody(Interview interview) => $@"
<p>Dear {interview.JobApplication?.CandidateName},</p>
<p>Your interview has been rescheduled to a new date/time:</p>
<p><strong>Date/Time:</strong> {interview.ScheduledStartAt:dddd, dd MMM yyyy HH:mm} - {interview.ScheduledEndAt:HH:mm}</p>
{LocationLine(interview)}
<p>Regards,<br/>SylviaNG Recruitment</p>";

        private static string BuildCancelledBody(Interview interview) => $@"
<p>Dear {interview.JobApplication?.CandidateName},</p>
<p>Your interview originally scheduled for {interview.ScheduledStartAt:dddd, dd MMM yyyy HH:mm} has been cancelled.</p>
{(string.IsNullOrWhiteSpace(interview.CancellationReason) ? string.Empty : $"<p><strong>Reason:</strong> {interview.CancellationReason}</p>")}
<p>Regards,<br/>SylviaNG Recruitment</p>";

        private static string BuildScheduledSms(Interview interview) =>
            $"Your interview is scheduled on {interview.ScheduledStartAt:dd MMM yyyy HH:mm}.";

        private static string BuildRescheduledSms(Interview interview) =>
            $"Your interview has been rescheduled to {interview.ScheduledStartAt:dd MMM yyyy HH:mm}.";

        private static string BuildCancelledSms(Interview interview) =>
            $"Your interview scheduled on {interview.ScheduledStartAt:dd MMM yyyy HH:mm} has been cancelled.";
    }
}
