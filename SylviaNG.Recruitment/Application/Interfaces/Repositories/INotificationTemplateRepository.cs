using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface INotificationTemplateRepository : IRepository<NotificationTemplate>
    {
        Task<bool> ExistsByCodeAsync(string code, long? excludeId = null);
        Task<List<NotificationTemplate>> GetAllOrderedAsync();
        Task<int> CountMappingUsageAsync(long notificationTemplateId);
        Task AddVersionAsync(NotificationTemplateVersion version);
        Task<List<NotificationTemplateVersion>> GetVersionsOrderedAsync(long notificationTemplateId);
    }
}
