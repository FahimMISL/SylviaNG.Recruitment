using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Commands.InterviewRoundConfigReplace;
using SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Models;
using SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Queries.InterviewRoundConfigGetAllByJobPosting;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>Named, sequenced interview rounds configured per job posting (EP-08 US-070).</summary>
    [ApiController]
    [Route("recruitment/job-posting/{jobPostingId}/interview-round-config")]
    [Authorize(Roles = "Admin,HR")]
    public class InterviewRoundConfigController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InterviewRoundConfigController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>All rounds configured for this job posting, ordered by sequence.</summary>
        [HttpGet]
        public async Task<ActionResult<List<InterviewRoundConfigResponse>>> GetAll(long jobPostingId)
        {
            var result = await _mediator.Send(new InterviewRoundConfigGetAllByJobPostingQuery(jobPostingId));
            return Ok(result);
        }

        /// <summary>Replaces every round configured for this job posting with the submitted list.</summary>
        [HttpPut]
        public async Task<ActionResult> Replace(long jobPostingId, [FromBody] InterviewRoundConfigReplaceRequest request)
        {
            await _mediator.Send(new InterviewRoundConfigReplaceCommand(jobPostingId, request));
            return Ok();
        }
    }
}
