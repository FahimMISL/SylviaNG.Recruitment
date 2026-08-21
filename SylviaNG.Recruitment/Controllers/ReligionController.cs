using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Religions.Commands.ReligionCreate;
using SylviaNG.Recruitment.Application.Features.Religions.Commands.ReligionDelete;
using SylviaNG.Recruitment.Application.Features.Religions.Commands.ReligionUpdate;
using SylviaNG.Recruitment.Application.Features.Religions.Models;
using SylviaNG.Recruitment.Application.Features.Religions.Queries.ReligionGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // GetAll has no [Authorize] - global AuthorizeFilter already requires login, and any
    // authenticated role (candidate filling the personal info form) needs this lookup. Writes
    // are Admin-only (System Administration / master data management).
    [ApiController]
    [Route("recruitment/religion")]
    public class ReligionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReligionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReligionResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ReligionGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] ReligionCreateRequest request)
        {
            var id = await _mediator.Send(new ReligionCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{religionId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long religionId, [FromBody] ReligionUpdateRequest request)
        {
            await _mediator.Send(new ReligionUpdateCommand(religionId, request));
            return Ok();
        }

        [HttpDelete("{religionId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long religionId)
        {
            await _mediator.Send(new ReligionDeleteCommand(religionId));
            return Ok();
        }
    }
}
