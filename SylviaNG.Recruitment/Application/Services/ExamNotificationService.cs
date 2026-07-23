using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Composes and sends the enrollment email (with admit-card PDF attached) and SMS summary
    /// for a single ExamEnrollment (US-055/US-056). Deliberately never throws - every risky
    /// section is wrapped so a mail-server or SMS-gateway failure never blocks the caller's bulk
    /// enroll action; the outcome is written onto the enrollment row instead.
    /// </summary>
    public class ExamNotificationService : IExamNotificationService
    {
        private readonly IExamEnrollmentRepository _examEnrollmentRepository;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly ISmsNotificationService _smsNotificationService;
        private readonly IAdmitCardPdfGeneratorService _admitCardPdfGeneratorService;
        private readonly PortalSettings _portalSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExamNotificationService> _logger;

        public ExamNotificationService(
            IExamEnrollmentRepository examEnrollmentRepository,
            ISmtpEmailService smtpEmailService,
            ISmsNotificationService smsNotificationService,
            IAdmitCardPdfGeneratorService admitCardPdfGeneratorService,
            IOptions<PortalSettings> portalSettings,
            IUnitOfWork unitOfWork,
            ILogger<ExamNotificationService> logger)
        {
            _examEnrollmentRepository = examEnrollmentRepository;
            _smtpEmailService = smtpEmailService;
            _smsNotificationService = smsNotificationService;
            _admitCardPdfGeneratorService = admitCardPdfGeneratorService;
            _portalSettings = portalSettings.Value;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>US-057 AC2/AC3: re-runs the same per-enrollment notify path used at
        /// enrollment time for every enrollment in the exam, in one HR-triggered action.</summary>
        public async Task<(int EmailSentCount, int SmsSentCount, int TotalCount)> DistributeBulkAsync(long examId)
        {
            var enrollments = await _examEnrollmentRepository.GetByExamIdWithDetailsAsync(examId);

            foreach (var enrollment in enrollments)
            {
                await NotifyEnrollmentAsync(enrollment, enrollment.Exam, enrollment.JobApplication);
            }

            return (
                enrollments.Count(e => e.EmailNotificationStatus == NotificationStatusEnum.Sent),
                enrollments.Count(e => e.SmsNotificationStatus == NotificationStatusEnum.Sent),
                enrollments.Count);
        }

        public async Task NotifyEnrollmentAsync(ExamEnrollment enrollment, Exam exam, JobApplication jobApplication)
        {
            try
            {
                await SendEmailAsync(enrollment, exam, jobApplication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending exam enrollment email for ExamEnrollmentId {ExamEnrollmentId}.", enrollment.ExamEnrollmentId);
                enrollment.EmailNotificationStatus = NotificationStatusEnum.Failed;
                enrollment.EmailFailureReason = ex.Message;
            }

            try
            {
                await SendSmsAsync(enrollment, exam, jobApplication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending exam enrollment SMS for ExamEnrollmentId {ExamEnrollmentId}.", enrollment.ExamEnrollmentId);
                enrollment.SmsNotificationStatus = NotificationStatusEnum.Failed;
            }

            try
            {
                _examEnrollmentRepository.Update(enrollment);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist notification status for ExamEnrollmentId {ExamEnrollmentId}.", enrollment.ExamEnrollmentId);
            }
        }

        private async Task SendEmailAsync(ExamEnrollment enrollment, Exam exam, JobApplication jobApplication)
        {
            if (string.IsNullOrWhiteSpace(jobApplication.CandidateEmail))
            {
                enrollment.EmailNotificationStatus = NotificationStatusEnum.Skipped;
                return;
            }

            var htmlBody = BuildEmailBody(enrollment, exam, jobApplication);
            var admitCardPdf = _admitCardPdfGeneratorService.Generate(enrollment, exam, jobApplication);

            var message = new EmailMessage
            {
                To = jobApplication.CandidateEmail,
                Subject = $"Exam Admit Card - {exam.Title}",
                HtmlBody = htmlBody,
                Attachments = new List<EmailAttachment>
                {
                    new EmailAttachment
                    {
                        FileName = $"Admit-Card-{jobApplication.JobApplicationId}.pdf",
                        ContentType = "application/pdf",
                        Content = admitCardPdf
                    }
                }
            };

            var result = await _smtpEmailService.TrySendAsync(message);

            if (result.Success)
            {
                enrollment.EmailNotificationStatus = NotificationStatusEnum.Sent;
                enrollment.EmailSentAt = DateTime.UtcNow;
                enrollment.EmailFailureReason = null;
            }
            else
            {
                enrollment.EmailNotificationStatus = NotificationStatusEnum.Failed;
                enrollment.EmailFailureReason = result.ErrorMessage;
            }
        }

        private async Task SendSmsAsync(ExamEnrollment enrollment, Exam exam, JobApplication jobApplication)
        {
            if (string.IsNullOrWhiteSpace(jobApplication.CandidatePhone))
            {
                enrollment.SmsNotificationStatus = NotificationStatusEnum.Skipped;
                return;
            }

            var summary = $"Your exam '{exam.Title}' is scheduled on {exam.ScheduledStartAt:dd MMM yyyy HH:mm}. " +
                (string.IsNullOrWhiteSpace(enrollment.SeatNumber) ? "Seat to be assigned. " : $"Seat: {enrollment.SeatNumber}. ") +
                $"Download your admit card at {_portalSettings.FrontendBaseUrl}/my-applications.";

            var sent = await _smsNotificationService.TrySendAsync(jobApplication.CandidatePhone, summary);

            if (sent)
            {
                enrollment.SmsNotificationStatus = NotificationStatusEnum.Sent;
                enrollment.SmsLoggedAt = DateTime.UtcNow;
            }
            else
            {
                enrollment.SmsNotificationStatus = NotificationStatusEnum.Failed;
            }
        }

        private static string BuildEmailBody(ExamEnrollment enrollment, Exam exam, JobApplication jobApplication)
        {
            var venueLine = exam.ExamVenue != null
                ? $"<p><strong>Venue:</strong> {exam.ExamVenue.VenueName} - {exam.ExamVenue.Location}</p>"
                : string.Empty;

            var seatLine = !string.IsNullOrWhiteSpace(enrollment.SeatNumber)
                ? $"<p><strong>Seat Number:</strong> {enrollment.SeatNumber}</p>"
                : string.Empty;

            return $@"
<p>Dear {jobApplication.CandidateName},</p>
<p>You have been enrolled for the following exam:</p>
<p><strong>Exam:</strong> {exam.Title}</p>
<p><strong>Date/Time:</strong> {exam.ScheduledStartAt:dddd, dd MMM yyyy HH:mm}</p>
<p><strong>Duration:</strong> {exam.DurationMinutes} minutes</p>
{venueLine}
{seatLine}
<p>Your admit card is attached to this email. Please bring a valid photo ID on the day of the exam.</p>
<p>Regards,<br/>SylviaNG Recruitment</p>";
        }
    }
}
