using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Composes and sends the enrollment email (with admit-card PDF attached) and SMS summary
    /// for a single ExamEnrollment (US-055/US-056). Deliberately never throws - every risky
    /// section is wrapped so a mail-server or SMS-gateway failure never blocks the caller's bulk
    /// enroll action; the outcome is written onto the enrollment row instead. Email routes through
    /// INotificationDispatchService (RecruitmentEventEnum.AdmitCardIssued) so its copy lives in the
    /// same DB-editable template system as every other notification, not a hardcoded C# string.
    /// </summary>
    public class ExamNotificationService : IExamNotificationService
    {
        private readonly IExamEnrollmentRepository _examEnrollmentRepository;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly ISmsNotificationService _smsNotificationService;
        private readonly IAdmitCardPdfGeneratorService _admitCardPdfGeneratorService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly PortalSettings _portalSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExamNotificationService> _logger;

        public ExamNotificationService(
            IExamEnrollmentRepository examEnrollmentRepository,
            INotificationDispatchService notificationDispatchService,
            ISmsNotificationService smsNotificationService,
            IAdmitCardPdfGeneratorService admitCardPdfGeneratorService,
            IApplicationSettingService applicationSettingService,
            IOptions<PortalSettings> portalSettings,
            IUnitOfWork unitOfWork,
            ILogger<ExamNotificationService> logger)
        {
            _examEnrollmentRepository = examEnrollmentRepository;
            _notificationDispatchService = notificationDispatchService;
            _smsNotificationService = smsNotificationService;
            _admitCardPdfGeneratorService = admitCardPdfGeneratorService;
            _applicationSettingService = applicationSettingService;
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

            var admitCardPdf = await _admitCardPdfGeneratorService.Generate(enrollment, exam, jobApplication);

            var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = jobApplication.CandidateName,
                ["ExamTitle"] = exam.Title,
                // ScheduledStartAt is stored UTC (timestamptz) - every other display path
                // (API JSON responses) converts back to local via LocalDateTimeJsonConverter;
                // this hand-built email string bypassed that entirely and showed raw UTC.
                ["ScheduledStartAt"] = DateTimeUtility.ConvertUtcToLocal(exam.ScheduledStartAt).ToString("dddd, dd MMM yyyy hh:mm tt"),
                ["DurationMinutes"] = exam.DurationMinutes.ToString(),
                ["VenueName"] = exam.ExamVenue?.VenueName ?? string.Empty,
                ["VenueLocation"] = exam.ExamVenue?.Location ?? string.Empty,
                ["SeatNumber"] = enrollment.SeatNumber ?? string.Empty
            };

            var attachments = new List<EmailAttachment>
            {
                new EmailAttachment
                {
                    FileName = $"Admit-Card-{jobApplication.JobApplicationId}.pdf",
                    ContentType = "application/pdf",
                    Content = admitCardPdf
                }
            };

            var dispatchResult = await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.AdmitCardIssued,
                placeholders,
                new NotificationDispatchTargets(jobApplication.CandidateEmail, await _applicationSettingService.GetHrNotificationEmailAsync(), jobApplication.JobApplicationId, NotifyActiveHrUsers: true),
                attachments: attachments);

            if (dispatchResult.CandidateResult?.Success == true)
            {
                enrollment.EmailNotificationStatus = NotificationStatusEnum.Sent;
                enrollment.EmailSentAt = DateTime.UtcNow;
                enrollment.EmailFailureReason = null;
            }
            else
            {
                enrollment.EmailNotificationStatus = NotificationStatusEnum.Failed;
                enrollment.EmailFailureReason = dispatchResult.CandidateResult?.ErrorMessage ?? "Unknown error";
            }
        }

        private async Task SendSmsAsync(ExamEnrollment enrollment, Exam exam, JobApplication jobApplication)
        {
            if (string.IsNullOrWhiteSpace(jobApplication.CandidatePhone))
            {
                enrollment.SmsNotificationStatus = NotificationStatusEnum.Skipped;
                return;
            }

            var summary = $"Your exam '{exam.Title}' is scheduled on {DateTimeUtility.ConvertUtcToLocal(exam.ScheduledStartAt):dd MMM yyyy hh:mm tt}. " +
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

    }
}
