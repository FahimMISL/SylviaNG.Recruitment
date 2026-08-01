using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface INotificationLogService
    {
        Task<PagedResult<NotificationLogResponse>> GetFilteredAsync(NotificationLogFilterRequest filter);
        Task<NotificationLogFileResponse> ExportExcelAsync(NotificationLogFilterRequest filter);
        Task<List<NotificationLogResponse>> GetUnreadAsync();
        Task<int> GetUnreadCountAsync();
        Task<NotificationLogResponse> RetryAsync(long notificationLogId);
        Task MarkAsReadAsync(long notificationLogId);
        Task<int> MarkAllAsReadAsync();

        /// <summary>Candidate-facing bell, scoped to the current caller's own CandidateProfile.</summary>
        Task<List<NotificationLogResponse>> GetUnreadForCurrentCandidateAsync();
        Task<int> GetUnreadCountForCurrentCandidateAsync();
        Task<int> MarkAllAsReadForCurrentCandidateAsync();

        /// <summary>Throws NotFoundException if the row doesn't belong to the current candidate -
        /// same "don't leak existence" shape as a straight 404, no separate 403 needed here.</summary>
        Task MarkAsReadForCurrentCandidateAsync(long notificationLogId);
    }
}
