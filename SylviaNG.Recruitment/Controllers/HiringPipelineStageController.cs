using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Queries.HiringPipelineStageGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Commands.HiringPipelineStageCreate;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Commands.HiringPipelineStageDelete;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Commands.HiringPipelineStageUpdate;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Queries.HiringPipelineStageGetAll;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Queries.HiringPipelineStageGetById;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Queries.HiringPipelineStageGetByJobPostingId;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/hiring-pipeline-stage")]
    public class HiringPipelineStageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HiringPipelineStageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<HiringPipelineStageResponse>>> GetAll()
        {
            var result = await _mediator.Send(new HiringPipelineStageGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<HiringPipelineStageResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new HiringPipelineStageGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{hiringPipelineStageId}")]
        public async Task<ActionResult<HiringPipelineStageResponse>> GetById(long hiringPipelineStageId)
        {
            var result = await _mediator.Send(new HiringPipelineStageGetByIdQuery(hiringPipelineStageId));
            return Ok(result);
        }

        [HttpGet("by-job/{jobPostingId}")]
        public async Task<ActionResult<List<HiringPipelineStageResponse>>> GetByJobPostingId(long jobPostingId)
        {
            var result = await _mediator.Send(new HiringPipelineStageGetByJobPostingIdQuery(jobPostingId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] HiringPipelineStageCreateRequest request)
        {
            var id = await _mediator.Send(new HiringPipelineStageCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{hiringPipelineStageId}")]
        public async Task<ActionResult> Update(long hiringPipelineStageId, [FromBody] HiringPipelineStageUpdateRequest request)
        {
            await _mediator.Send(new HiringPipelineStageUpdateCommand(hiringPipelineStageId, request));
            return Ok();
        }

        [HttpDelete("{hiringPipelineStageId}")]
        public async Task<ActionResult> Delete(long hiringPipelineStageId)
        {
            await _mediator.Send(new HiringPipelineStageDeleteCommand(hiringPipelineStageId));
            return Ok();
        }
    }
}
