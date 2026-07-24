using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.MaritalStatuses.Commands.MaritalStatusCreate;
using SylviaNG.Recruitment.Application.Features.MaritalStatuses.Commands.MaritalStatusDelete;
using SylviaNG.Recruitment.Application.Features.MaritalStatuses.Commands.MaritalStatusUpdate;
using SylviaNG.Recruitment.Application.Features.MaritalStatuses.Models;
using SylviaNG.Recruitment.Application.Features.MaritalStatuses.Queries.MaritalStatusGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // GetAll has no [Authorize] - global AuthorizeFilter already requires login, and any
    // authenticated role (candidate filling the personal info form) needs this lookup. Writes
    // are Admin-only (System Administration / master data management).
    [ApiController]
    [Route("recruitment/marital-status")]
    public class MaritalStatusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MaritalStatusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<MaritalStatusResponse>>> GetAll()
        {
            var result = await _mediator.Send(new MaritalStatusGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] MaritalStatusCreateRequest request)
        {
            var id = await _mediator.Send(new MaritalStatusCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{maritalStatusId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long maritalStatusId, [FromBody] MaritalStatusUpdateRequest request)
        {
            await _mediator.Send(new MaritalStatusUpdateCommand(maritalStatusId, request));
            return Ok();
        }

        [HttpDelete("{maritalStatusId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long maritalStatusId)
        {
            await _mediator.Send(new MaritalStatusDeleteCommand(maritalStatusId));
            return Ok();
        }
    }
}
