using SylviaNG.Recruitment.Application.Features.Dashboard.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<DashboardSummaryResponse> GetSummaryAsync();
    }
}
