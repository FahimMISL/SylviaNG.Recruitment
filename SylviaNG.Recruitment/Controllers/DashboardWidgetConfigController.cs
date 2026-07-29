using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Dashboard.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>
    /// EP-14 US-105 AC5 (minimal build ahead of full EP-15 access control): per-role dashboard
    /// widget visibility. GetVisible is any authenticated Admin/HR (the dashboard reads its own
    /// role's visible set); the config list and the toggle are Admin-only.
    /// </summary>
    [ApiController]
    [Route("recruitment/dashboard/widget-config")]
    [Authorize(Roles = "Admin,HR")]
    public class DashboardWidgetConfigController : ControllerBase
    {
        private readonly IDashboardWidgetConfigService _dashboardWidgetConfigService;

        public DashboardWidgetConfigController(IDashboardWidgetConfigService dashboardWidgetConfigService)
        {
            _dashboardWidgetConfigService = dashboardWidgetConfigService;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> GetVisibleForCurrentRole()
        {
            var result = await _dashboardWidgetConfigService.GetVisibleWidgetKeysForCurrentRoleAsync();
            return Ok(result);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DashboardWidgetConfigResponse>>> GetAll()
        {
            var result = await _dashboardWidgetConfigService.GetAllAsync();
            return Ok(result);
        }

        [HttpPatch("{widgetKey}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateVisibility(string widgetKey, [FromBody] DashboardWidgetConfigUpdateRequest request)
        {
            await _dashboardWidgetConfigService.UpdateVisibilityAsync(widgetKey, request);
            return Ok();
        }
    }
}
