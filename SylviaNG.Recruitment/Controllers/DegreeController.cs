using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Degrees.Commands.DegreeCreate;
using SylviaNG.Recruitment.Application.Features.Degrees.Commands.DegreeDelete;
using SylviaNG.Recruitment.Application.Features.Degrees.Commands.DegreeUpdate;
using SylviaNG.Recruitment.Application.Features.Degrees.Models;
using SylviaNG.Recruitment.Application.Features.Degrees.Queries.DegreeGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // GetAll has no [Authorize] - global AuthorizeFilter already requires login, and any
    // authenticated role (candidate filling the education form's Degree dropdown) needs this
    // lookup. Writes are Admin-only (System Administration / master data management, including
    // setting the Position equivalence group).
    [ApiController]
    [Route("recruitment/degree")]
    public class DegreeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DegreeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<DegreeResponse>>> GetAll()
        {
            var result = await _mediator.Send(new DegreeGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] DegreeCreateRequest request)
        {
            var id = await _mediator.Send(new DegreeCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{degreeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long degreeId, [FromBody] DegreeUpdateRequest request)
        {
            await _mediator.Send(new DegreeUpdateCommand(degreeId, request));
            return Ok();
        }

        [HttpDelete("{degreeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long degreeId)
        {
            await _mediator.Send(new DegreeDeleteCommand(degreeId));
            return Ok();
        }
    }
}
