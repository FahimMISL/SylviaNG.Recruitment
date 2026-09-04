using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Genders.Commands.GenderCreate;
using SylviaNG.Recruitment.Application.Features.Genders.Commands.GenderDelete;
using SylviaNG.Recruitment.Application.Features.Genders.Commands.GenderUpdate;
using SylviaNG.Recruitment.Application.Features.Genders.Models;
using SylviaNG.Recruitment.Application.Features.Genders.Queries.GenderGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // GetAll has no [Authorize] - global AuthorizeFilter already requires login, and any
    // authenticated role (candidate filling the personal info form) needs this lookup. Writes
    // are Admin-only (System Administration / master data management).
    [ApiController]
    [Route("recruitment/gender")]
    public class GenderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GenderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenderResponse>>> GetAll()
        {
            var result = await _mediator.Send(new GenderGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] GenderCreateRequest request)
        {
            var id = await _mediator.Send(new GenderCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{genderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long genderId, [FromBody] GenderUpdateRequest request)
        {
            await _mediator.Send(new GenderUpdateCommand(genderId, request));
            return Ok();
        }

        [HttpDelete("{genderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long genderId)
        {
            await _mediator.Send(new GenderDeleteCommand(genderId));
            return Ok();
        }
    }
}
