using SylviaNG.Recruitment.Application.Features.Dashboard.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IDashboardWidgetConfigService
    {
        /// <summary>Widget keys visible to the current caller's role (Admin or HR) - drives which
        /// cards DashboardService.GetSummaryAsync includes.</summary>
        Task<List<string>> GetVisibleWidgetKeysForCurrentRoleAsync();

        /// <summary>Full config for all 5 widgets, Admin-only, for the visibility-toggle UI.</summary>
        Task<List<DashboardWidgetConfigResponse>> GetAllAsync();

        /// <summary>Admin-only toggle of a single widget's per-role visibility.</summary>
        Task UpdateVisibilityAsync(string widgetKey, DashboardWidgetConfigUpdateRequest request);
    }
}
