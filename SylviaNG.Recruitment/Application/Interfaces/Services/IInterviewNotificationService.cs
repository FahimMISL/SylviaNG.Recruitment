using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>Never throws - failures are captured onto the interview's notification status fields.</summary>
    public interface IInterviewNotificationService
    {
        Task NotifyScheduledAsync(Interview interview);
        Task NotifyRescheduledAsync(Interview interview);
        Task NotifyCancelledAsync(Interview interview);
    }
}
