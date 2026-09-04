using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ReferralSources.Commands.ReferralSourceCreate;
using SylviaNG.Recruitment.Application.Features.ReferralSources.Commands.ReferralSourceDelete;
using SylviaNG.Recruitment.Application.Features.ReferralSources.Commands.ReferralSourceUpdate;
using SylviaNG.Recruitment.Application.Features.ReferralSources.Models;
using SylviaNG.Recruitment.Application.Features.ReferralSources.Queries.ReferralSourceGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // GetAll has no [Authorize] - global AuthorizeFilter already requires login, and the apply
    // form's optional Referral Source dropdown needs this lookup for any authenticated candidate.
    // Writes are Admin-only (System Administration / master data management).
    [ApiController]
    [Route("recruitment/referral-source")]
    public class ReferralSourceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReferralSourceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReferralSourceResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ReferralSourceGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] ReferralSourceCreateRequest request)
        {
            var id = await _mediator.Send(new ReferralSourceCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{referralSourceId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long referralSourceId, [FromBody] ReferralSourceUpdateRequest request)
        {
            await _mediator.Send(new ReferralSourceUpdateCommand(referralSourceId, request));
            return Ok();
        }

        [HttpDelete("{referralSourceId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long referralSourceId)
        {
            await _mediator.Send(new ReferralSourceDeleteCommand(referralSourceId));
            return Ok();
        }
    }
}
