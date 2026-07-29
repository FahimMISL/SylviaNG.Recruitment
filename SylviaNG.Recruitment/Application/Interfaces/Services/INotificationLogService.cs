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
    }
}
