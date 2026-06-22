using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface INotificationEventService
    {
        Task<long> CreateAsync(NotificationEventCreateRequest request);
        Task UpdateAsync(long notificationEventId, NotificationEventUpdateRequest request);
        Task DeleteAsync(long notificationEventId);
        Task<List<NotificationEventResponse>> GetAllAsync();
        Task<NotificationEventResponse> GetByIdAsync(long notificationEventId);
        Task<PagedResult<NotificationEventResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
