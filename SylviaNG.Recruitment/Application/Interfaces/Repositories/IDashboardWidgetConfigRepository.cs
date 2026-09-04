using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IDashboardWidgetConfigRepository : IRepository<DashboardWidgetConfig>
    {
        Task<List<DashboardWidgetConfig>> GetAllOrderedAsync();
        Task<DashboardWidgetConfig?> GetByKeyAsync(string widgetKey);
    }
}
