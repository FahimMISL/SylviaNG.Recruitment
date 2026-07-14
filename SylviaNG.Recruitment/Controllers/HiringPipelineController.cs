using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineCreate;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineDelete;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineDuplicate;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineSetActive;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineUpdate;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Queries.HiringPipelineGetActiveLookup;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Queries.HiringPipelineGetAll;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Queries.HiringPipelineGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/hiring-pipeline")]
    [Authorize(Roles = "Admin,HR")]
    public class HiringPipelineController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HiringPipelineController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all hiring pipelines with their stages.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<HiringPipelineResponse>>> GetAll()
        {
            var result = await _mediator.Send(new HiringPipelineGetAllQuery());
            return Ok(result);
        }

        /// <summary>
        /// Active pipelines only (Id + Name), for the "assign pipeline to job posting" dropdown.
        /// </summary>
        [HttpGet("active-lookup")]
        public async Task<ActionResult<List<HiringPipelineLookupResponse>>> GetActiveLookup()
        {
            var result = await _mediator.Send(new HiringPipelineGetActiveLookupQuery());
            return Ok(result);
        }

        /// <summary>
        /// Get a hiring pipeline by ID, including its ordered stages.
        /// </summary>
        [HttpGet("{hiringPipelineId}")]
        public async Task<ActionResult<HiringPipelineResponse>> GetById(long hiringPipelineId)
        {
            var result = await _mediator.Send(new HiringPipelineGetByIdQuery(hiringPipelineId));
            return Ok(result);
        }

        /// <summary>
        /// Create a new hiring pipeline with its stages.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] HiringPipelineCreateRequest request)
        {
            var id = await _mediator.Send(new HiringPipelineCreateCommand(request));
            return Ok(id);
        }

        /// <summary>
        /// Update a hiring pipeline. The stage list is a full replacement (add/edit/remove/reorder in one save).
        /// </summary>
        [HttpPut("{hiringPipelineId}")]
        public async Task<ActionResult> Update(long hiringPipelineId, [FromBody] HiringPipelineUpdateRequest request)
        {
            await _mediator.Send(new HiringPipelineUpdateCommand(hiringPipelineId, request));
            return Ok();
        }

        /// <summary>
        /// Delete a hiring pipeline. Fails with 409 if any job posting is still assigned to it.
        /// </summary>
        [HttpDelete("{hiringPipelineId}")]
        public async Task<ActionResult> Delete(long hiringPipelineId)
        {
            await _mediator.Send(new HiringPipelineDeleteCommand(hiringPipelineId));
            return Ok();
        }

        /// <summary>
        /// Clone a pipeline (and all its stages) as a new, inactive pipeline named "{Name} (Copy)".
        /// </summary>
        [HttpPost("{hiringPipelineId}/duplicate")]
        public async Task<ActionResult<long>> Duplicate(long hiringPipelineId)
        {
            var newId = await _mediator.Send(new HiringPipelineDuplicateCommand(hiringPipelineId));
            return Ok(newId);
        }

        /// <summary>
        /// Activate or deactivate a pipeline without touching its stages.
        /// </summary>
        [HttpPatch("{hiringPipelineId}/active")]
        public async Task<ActionResult> SetActive(long hiringPipelineId, [FromQuery] bool isActive)
        {
            await _mediator.Send(new HiringPipelineSetActiveCommand(hiringPipelineId, isActive));
            return Ok();
        }
    }
}
