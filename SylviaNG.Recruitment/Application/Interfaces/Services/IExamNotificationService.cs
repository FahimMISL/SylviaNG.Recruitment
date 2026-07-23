using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>Never throws - failures are captured onto the enrollment's notification status fields.</summary>
    public interface IExamNotificationService
    {
        Task NotifyEnrollmentAsync(ExamEnrollment enrollment, Exam exam, JobApplication jobApplication);
    }
}
