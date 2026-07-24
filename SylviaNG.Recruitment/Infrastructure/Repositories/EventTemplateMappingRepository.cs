using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class EventTemplateMappingRepository : Repository<EventTemplateMapping>, IEventTemplateMappingRepository
    {
        public EventTemplateMappingRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsAsync(RecruitmentEventEnum recruitmentEvent, NotificationChannelEnum channel, NotificationRecipientTypeEnum recipientType, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(m =>
                m.RecruitmentEvent == recruitmentEvent &&
                m.Channel == channel &&
                m.RecipientType == recipientType &&
                (!excludeId.HasValue || m.EventTemplateMappingId != excludeId.Value));
        }

        public async Task<List<EventTemplateMapping>> GetAllWithTemplateAsync()
        {
            return await _dbSet
                .Include(m => m.NotificationTemplate)
                .OrderBy(m => m.RecruitmentEvent)
                .ThenBy(m => m.Channel)
                .ThenBy(m => m.RecipientType)
                .ToListAsync();
        }

        public async Task<EventTemplateMapping?> GetByIdWithTemplateAsync(long eventTemplateMappingId)
        {
            return await _dbSet
                .Include(m => m.NotificationTemplate)
                .FirstOrDefaultAsync(m => m.EventTemplateMappingId == eventTemplateMappingId);
        }
    }
}
