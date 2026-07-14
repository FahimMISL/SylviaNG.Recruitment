using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.PipelineProgress.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>Candidate pipeline progress tracker (US-042) - a sub-resource of JobApplication.</summary>
    [ApiController]
    [Route("recruitment/job-application/{jobApplicationId}/pipeline-progress")]
    [Authorize(Roles = "Admin,HR")]
    public class JobApplicationStageProgressController : ControllerBase
    {
        private readonly IJobApplicationStageProgressService _stageProgressService;

        public JobApplicationStageProgressController(IJobApplicationStageProgressService stageProgressService)
        {
            _stageProgressService = stageProgressService;
        }

        /// <summary>
        /// The tracker for one application: every configured pipeline stage as a card with its
        /// own status, schedule, meeting link, and notes (US-042 AC1/AC2/AC5).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<JobApplicationPipelineProgressResponse>> Get(long jobApplicationId)
        {
            var result = await _stageProgressService.GetByJobApplicationIdAsync(jobApplicationId);
            return Ok(result);
        }

        /// <summary>Start/complete/schedule/reschedule/add notes to one stage card (US-042 AC3).</summary>
        [HttpPatch("{pipelineStageId}")]
        public async Task<ActionResult> UpdateStage(long jobApplicationId, long pipelineStageId, [FromBody] PipelineStageProgressUpdateRequest request)
        {
            await _stageProgressService.UpdateStageAsync(jobApplicationId, pipelineStageId, request);
            return Ok();
        }
    }
}
