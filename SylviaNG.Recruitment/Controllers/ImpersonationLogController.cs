using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Queries.ImpersonationLogGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Commands.ImpersonationLogCreate;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Commands.ImpersonationLogDelete;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Commands.ImpersonationLogUpdate;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Queries.ImpersonationLogGetAll;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Queries.ImpersonationLogGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("recruitment/impersonation-log")]
    public class ImpersonationLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ImpersonationLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ImpersonationLogResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ImpersonationLogGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ImpersonationLogResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ImpersonationLogGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{impersonationLogId}")]
        public async Task<ActionResult<ImpersonationLogResponse>> GetById(long impersonationLogId)
        {
            var result = await _mediator.Send(new ImpersonationLogGetByIdQuery(impersonationLogId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ImpersonationLogCreateRequest request)
        {
            var id = await _mediator.Send(new ImpersonationLogCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{impersonationLogId}")]
        public async Task<ActionResult> Update(long impersonationLogId, [FromBody] ImpersonationLogUpdateRequest request)
        {
            await _mediator.Send(new ImpersonationLogUpdateCommand(impersonationLogId, request));
            return Ok();
        }

        [HttpDelete("{impersonationLogId}")]
        public async Task<ActionResult> Delete(long impersonationLogId)
        {
            await _mediator.Send(new ImpersonationLogDeleteCommand(impersonationLogId));
            return Ok();
        }
    }
}
