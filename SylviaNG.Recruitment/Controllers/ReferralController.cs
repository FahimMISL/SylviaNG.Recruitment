using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Referrals.Queries.ReferralGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Referrals.Commands.ReferralCreate;
using SylviaNG.Recruitment.Application.Features.Referrals.Commands.ReferralDelete;
using SylviaNG.Recruitment.Application.Features.Referrals.Commands.ReferralUpdate;
using SylviaNG.Recruitment.Application.Features.Referrals.Models;
using SylviaNG.Recruitment.Application.Features.Referrals.Queries.ReferralGetAll;
using SylviaNG.Recruitment.Application.Features.Referrals.Queries.ReferralGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/referral")]
    public class ReferralController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReferralController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReferralResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ReferralGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ReferralResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ReferralGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{referralId}")]
        public async Task<ActionResult<ReferralResponse>> GetById(long referralId)
        {
            var result = await _mediator.Send(new ReferralGetByIdQuery(referralId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ReferralCreateRequest request)
        {
            var id = await _mediator.Send(new ReferralCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{referralId}")]
        public async Task<ActionResult> Update(long referralId, [FromBody] ReferralUpdateRequest request)
        {
            await _mediator.Send(new ReferralUpdateCommand(referralId, request));
            return Ok();
        }

        [HttpDelete("{referralId}")]
        public async Task<ActionResult> Delete(long referralId)
        {
            await _mediator.Send(new ReferralDeleteCommand(referralId));
            return Ok();
        }
    }
}
