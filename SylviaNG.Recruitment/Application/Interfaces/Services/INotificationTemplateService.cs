using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface INotificationTemplateService
    {
        Task<long> CreateAsync(NotificationTemplateCreateRequest request);
        Task UpdateAsync(long notificationTemplateId, NotificationTemplateUpdateRequest request);
        Task DeleteAsync(long notificationTemplateId);
        Task<List<NotificationTemplateResponse>> GetAllAsync();
        Task<NotificationTemplateResponse> GetByIdAsync(long notificationTemplateId);
        Task<List<NotificationTemplateVersionResponse>> GetVersionsAsync(long notificationTemplateId);
    }
}
