using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Queries.IntegrationLogGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Commands.IntegrationLogCreate;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Commands.IntegrationLogDelete;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Commands.IntegrationLogUpdate;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Queries.IntegrationLogGetAll;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Queries.IntegrationLogGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("recruitment/integration-log")]
    public class IntegrationLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IntegrationLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<IntegrationLogResponse>>> GetAll()
        {
            var result = await _mediator.Send(new IntegrationLogGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<IntegrationLogResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new IntegrationLogGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{integrationLogId}")]
        public async Task<ActionResult<IntegrationLogResponse>> GetById(long integrationLogId)
        {
            var result = await _mediator.Send(new IntegrationLogGetByIdQuery(integrationLogId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] IntegrationLogCreateRequest request)
        {
            var id = await _mediator.Send(new IntegrationLogCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{integrationLogId}")]
        public async Task<ActionResult> Update(long integrationLogId, [FromBody] IntegrationLogUpdateRequest request)
        {
            await _mediator.Send(new IntegrationLogUpdateCommand(integrationLogId, request));
            return Ok();
        }

        [HttpDelete("{integrationLogId}")]
        public async Task<ActionResult> Delete(long integrationLogId)
        {
            await _mediator.Send(new IntegrationLogDeleteCommand(integrationLogId));
            return Ok();
        }
    }
}
