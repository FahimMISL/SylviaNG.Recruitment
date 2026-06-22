using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Nominees.Queries.NomineeGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Nominees.Commands.NomineeCreate;
using SylviaNG.Recruitment.Application.Features.Nominees.Commands.NomineeDelete;
using SylviaNG.Recruitment.Application.Features.Nominees.Commands.NomineeUpdate;
using SylviaNG.Recruitment.Application.Features.Nominees.Models;
using SylviaNG.Recruitment.Application.Features.Nominees.Queries.NomineeGetAll;
using SylviaNG.Recruitment.Application.Features.Nominees.Queries.NomineeGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/nominee")]
    public class NomineeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NomineeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<NomineeResponse>>> GetAll()
        {
            var result = await _mediator.Send(new NomineeGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<NomineeResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new NomineeGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{nomineeId}")]
        public async Task<ActionResult<NomineeResponse>> GetById(long nomineeId)
        {
            var result = await _mediator.Send(new NomineeGetByIdQuery(nomineeId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] NomineeCreateRequest request)
        {
            var id = await _mediator.Send(new NomineeCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{nomineeId}")]
        public async Task<ActionResult> Update(long nomineeId, [FromBody] NomineeUpdateRequest request)
        {
            await _mediator.Send(new NomineeUpdateCommand(nomineeId, request));
            return Ok();
        }

        [HttpDelete("{nomineeId}")]
        public async Task<ActionResult> Delete(long nomineeId)
        {
            await _mediator.Send(new NomineeDeleteCommand(nomineeId));
            return Ok();
        }
    }
}
