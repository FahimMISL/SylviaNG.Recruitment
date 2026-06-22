using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface INotificationLogService
    {
        Task<long> CreateAsync(NotificationLogCreateRequest request);
        Task UpdateAsync(long notificationLogId, NotificationLogUpdateRequest request);
        Task DeleteAsync(long notificationLogId);
        Task<List<NotificationLogResponse>> GetAllAsync();
        Task<NotificationLogResponse> GetByIdAsync(long notificationLogId);
        Task<PagedResult<NotificationLogResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
