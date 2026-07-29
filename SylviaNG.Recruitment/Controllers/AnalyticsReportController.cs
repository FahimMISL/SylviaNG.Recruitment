using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Analytics.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>EP-14 US-106/US-107/US-108/US-110: recruitment funnel, time-to-hire, candidate
    /// source, and interview performance analytics reports.</summary>
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

        [HttpGet("candidate-source")]
        public async Task<ActionResult<CandidateSourceAnalyticsResponse>> GetCandidateSourceAnalytics([FromQuery] CandidateSourceAnalyticsRequest request)
        {
            var result = await _analyticsReportService.GetCandidateSourceAnalyticsAsync(request);
            return Ok(result);
        }

        [HttpGet("candidate-source/export")]
        public async Task<IActionResult> ExportCandidateSourceAnalytics([FromQuery] CandidateSourceAnalyticsRequest request)
        {
            var excelBytes = await _analyticsReportService.ExportCandidateSourceAnalyticsExcelAsync(request);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Candidate-Source-Analytics-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx");
        }

        [HttpGet("interview-performance")]
        public async Task<ActionResult<InterviewAnalyticsResponse>> GetInterviewAnalytics([FromQuery] InterviewAnalyticsRequest request)
        {
            var result = await _analyticsReportService.GetInterviewAnalyticsAsync(request);
            return Ok(result);
        }

        [HttpGet("interview-performance/export")]
        public async Task<IActionResult> ExportInterviewAnalytics([FromQuery] InterviewAnalyticsRequest request)
        {
            var excelBytes = await _analyticsReportService.ExportInterviewAnalyticsExcelAsync(request);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Interview-Analytics-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx");
        }
    }
}
