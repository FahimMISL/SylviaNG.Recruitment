using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.BloodGroups.Commands.BloodGroupCreate;
using SylviaNG.Recruitment.Application.Features.BloodGroups.Commands.BloodGroupDelete;
using SylviaNG.Recruitment.Application.Features.BloodGroups.Commands.BloodGroupUpdate;
using SylviaNG.Recruitment.Application.Features.BloodGroups.Models;
using SylviaNG.Recruitment.Application.Features.BloodGroups.Queries.BloodGroupGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // GetAll has no [Authorize] - global AuthorizeFilter already requires login, and any
    // authenticated role (candidate filling the personal info form) needs this lookup. Writes
    // are Admin-only (System Administration / master data management).
    [ApiController]
    [Route("recruitment/blood-group")]
    public class BloodGroupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BloodGroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<BloodGroupResponse>>> GetAll()
        {
            var result = await _mediator.Send(new BloodGroupGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] BloodGroupCreateRequest request)
        {
            var id = await _mediator.Send(new BloodGroupCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{bloodGroupId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long bloodGroupId, [FromBody] BloodGroupUpdateRequest request)
        {
            await _mediator.Send(new BloodGroupUpdateCommand(bloodGroupId, request));
            return Ok();
        }

        [HttpDelete("{bloodGroupId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long bloodGroupId)
        {
            await _mediator.Send(new BloodGroupDeleteCommand(bloodGroupId));
            return Ok();
        }
    }
}
