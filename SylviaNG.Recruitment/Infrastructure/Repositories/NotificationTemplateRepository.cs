using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class NotificationTemplateRepository : Repository<NotificationTemplate>, INotificationTemplateRepository
    {
        public NotificationTemplateRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByCodeAsync(string code, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(t => t.Code == code && (!excludeId.HasValue || t.NotificationTemplateId != excludeId.Value));
        }

        public async Task<List<NotificationTemplate>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<int> CountMappingUsageAsync(long notificationTemplateId)
        {
            return await _dbContext.EventTemplateMappings.CountAsync(m => m.NotificationTemplateId == notificationTemplateId);
        }

        public async Task AddVersionAsync(NotificationTemplateVersion version)
        {
            await _dbContext.NotificationTemplateVersions.AddAsync(version);
        }

        public async Task<List<NotificationTemplateVersion>> GetVersionsOrderedAsync(long notificationTemplateId)
        {
            return await _dbContext.NotificationTemplateVersions
                .Where(v => v.NotificationTemplateId == notificationTemplateId)
                .OrderByDescending(v => v.VersionNumber)
                .ToListAsync();
        }
    }
}
