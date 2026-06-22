using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Queries.ReferralDuplicateGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Commands.ReferralDuplicateCreate;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Commands.ReferralDuplicateDelete;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Commands.ReferralDuplicateUpdate;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Models;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Queries.ReferralDuplicateGetAll;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Queries.ReferralDuplicateGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/referral-duplicate")]
    public class ReferralDuplicateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReferralDuplicateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReferralDuplicateResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ReferralDuplicateGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ReferralDuplicateResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ReferralDuplicateGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{referralDuplicateId}")]
        public async Task<ActionResult<ReferralDuplicateResponse>> GetById(long referralDuplicateId)
        {
            var result = await _mediator.Send(new ReferralDuplicateGetByIdQuery(referralDuplicateId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ReferralDuplicateCreateRequest request)
        {
            var id = await _mediator.Send(new ReferralDuplicateCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{referralDuplicateId}")]
        public async Task<ActionResult> Update(long referralDuplicateId, [FromBody] ReferralDuplicateUpdateRequest request)
        {
            await _mediator.Send(new ReferralDuplicateUpdateCommand(referralDuplicateId, request));
            return Ok();
        }

        [HttpDelete("{referralDuplicateId}")]
        public async Task<ActionResult> Delete(long referralDuplicateId)
        {
            await _mediator.Send(new ReferralDuplicateDeleteCommand(referralDuplicateId));
            return Ok();
        }
    }
}
