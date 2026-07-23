using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.AutoShortlisting.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>AI-powered (or Manual, config-switched) candidate ranking and auto-shortlisting for a vacancy (US-046).</summary>
    [ApiController]
    [Route("recruitment/auto-shortlist")]
    [Authorize(Roles = "Admin,HR")]
    public class AutoShortlistController : ControllerBase
    {
        private readonly IAutoShortlistRunService _autoShortlistRunService;

        public AutoShortlistController(IAutoShortlistRunService autoShortlistRunService)
        {
            _autoShortlistRunService = autoShortlistRunService;
        }

        /// <summary>Score every application for a vacancy and persist a new run (AC1/AC2).</summary>
        [HttpPost("run")]
        public async Task<ActionResult<AutoShortlistRunResponse>> Run([FromBody] AutoShortlistRunRequest request)
        {
            var result = await _autoShortlistRunService.RunAsync(request);
            return Ok(result);
        }

        /// <summary>Most recent run for a vacancy, or null if none exists yet - no rescoring.</summary>
        [HttpGet("{jobPostingId}/latest")]
        public async Task<ActionResult<AutoShortlistRunResponse?>> GetLatest(long jobPostingId)
        {
            var result = await _autoShortlistRunService.GetLatestAsync(jobPostingId);
            return Ok(result);
        }

        /// <summary>Adjust the cutoff score on an existing run (AC3/AC5) - no rescoring, no per-row writes.</summary>
        [HttpPatch("{runId}/cutoff")]
        public async Task<ActionResult<AutoShortlistRunResponse>> AdjustCutoff(long runId, [FromBody] AutoShortlistCutoffUpdateRequest request)
        {
            var result = await _autoShortlistRunService.AdjustCutoffAsync(runId, request.CutoffScore);
            return Ok(result);
        }

        /// <summary>Override one candidate's pass/fail decision, independent of score (AC5).</summary>
        [HttpPatch("result/{resultId}/override")]
        public async Task<ActionResult<AutoShortlistResultResponse>> Override(long resultId, [FromBody] AutoShortlistOverrideRequest request)
        {
            var result = await _autoShortlistRunService.OverrideAsync(resultId, request.Decision);
            return Ok(result);
        }

        /// <summary>Move every currently-included candidate to Shortlisted (AC3/AC4/AC5).</summary>
        [HttpPost("{runId}/apply")]
        public async Task<ActionResult<AutoShortlistApplyResponse>> Apply(long runId)
        {
            var result = await _autoShortlistRunService.ApplyAsync(runId);
            return Ok(result);
        }
    }
}
