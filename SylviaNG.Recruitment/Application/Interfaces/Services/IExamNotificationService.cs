using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>Never throws - failures are captured onto the enrollment's notification status fields.</summary>
    public interface IExamNotificationService
    {
        Task NotifyEnrollmentAsync(ExamEnrollment enrollment, Exam exam, JobApplication jobApplication);

        /// <summary>Re-sends the admit-card email+SMS to every enrollment in this exam in one
        /// action (US-057 AC2/AC3). Returns how many enrollments ended each notification
        /// channel in the Sent state, for the HR-facing summary.</summary>
        Task<(int EmailSentCount, int SmsSentCount, int TotalCount)> DistributeBulkAsync(long examId);
    }
}
