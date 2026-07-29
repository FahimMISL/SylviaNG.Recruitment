using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Analytics.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>EP-14 US-106/US-107: recruitment funnel + time-to-hire analytics reports.</summary>
    [ApiController]
    [Route("recruitment/analytics")]
    [Authorize(Roles = "Admin,HR")]
    public class AnalyticsReportController : ControllerBase
    {
        private readonly IAnalyticsReportService _analyticsReportService;

        public AnalyticsReportController(IAnalyticsReportService analyticsReportService)
        {
            _analyticsReportService = analyticsReportService;
        }

        [HttpGet("funnel")]
        public async Task<ActionResult<RecruitmentFunnelResponse>> GetFunnel([FromQuery] RecruitmentFunnelRequest request)
        {
            var result = await _analyticsReportService.GetRecruitmentFunnelAsync(request);
            return Ok(result);
        }

        [HttpGet("funnel/export")]
        public async Task<IActionResult> ExportFunnel([FromQuery] RecruitmentFunnelRequest request)
        {
            var csvBytes = await _analyticsReportService.ExportRecruitmentFunnelCsvAsync(request);
            return File(csvBytes, "text/csv", $"Recruitment-Funnel-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv");
        }

        [HttpGet("time-to-hire")]
        public async Task<ActionResult<TimeToHireResponse>> GetTimeToHire([FromQuery] TimeToHireRequest request)
        {
            var result = await _analyticsReportService.GetTimeToHireAsync(request);
            return Ok(result);
        }

        [HttpGet("time-to-hire/export")]
        public async Task<IActionResult> ExportTimeToHire([FromQuery] TimeToHireRequest request)
        {
            var csvBytes = await _analyticsReportService.ExportTimeToHireCsvAsync(request);
            return File(csvBytes, "text/csv", $"Time-To-Hire-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv");
        }
    }
}
