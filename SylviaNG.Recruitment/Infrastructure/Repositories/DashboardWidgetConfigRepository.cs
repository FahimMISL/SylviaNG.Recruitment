using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class DashboardWidgetConfigRepository : Repository<DashboardWidgetConfig>, IDashboardWidgetConfigRepository
    {
        public DashboardWidgetConfigRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<DashboardWidgetConfig>> GetAllOrderedAsync()
        {
            return await _dbSet.OrderBy(w => w.WidgetKey).ToListAsync();
        }

        public async Task<DashboardWidgetConfig?> GetByKeyAsync(string widgetKey)
        {
            return await _dbSet.FirstOrDefaultAsync(w => w.WidgetKey == widgetKey);
        }
    }
}
