using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.WaiverRules.Commands.WaiverRuleCreate;
using SylviaNG.Recruitment.Application.Features.WaiverRules.Commands.WaiverRuleDelete;
using SylviaNG.Recruitment.Application.Features.WaiverRules.Commands.WaiverRuleUpdate;
using SylviaNG.Recruitment.Application.Features.WaiverRules.Models;
using SylviaNG.Recruitment.Application.Features.WaiverRules.Queries.WaiverRuleGetAll;

namespace SylviaNG.Recruitment.Controllers
{
    // Admin-only end to end (EP-17/US-127) - unlike the master-data lookups (SpecialCategory,
    // ReferralSource), waiver rules aren't needed by any candidate-facing dropdown, so GetAll is
    // also locked down.
    [ApiController]
    [Route("recruitment/waiver-rule")]
    [Authorize(Roles = "Admin")]
    public class WaiverRuleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WaiverRuleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<WaiverRuleResponse>>> GetAll()
        {
            var result = await _mediator.Send(new WaiverRuleGetAllQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] WaiverRuleCreateRequest request)
        {
            var id = await _mediator.Send(new WaiverRuleCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{waiverRuleId}")]
        public async Task<ActionResult> Update(long waiverRuleId, [FromBody] WaiverRuleUpdateRequest request)
        {
            await _mediator.Send(new WaiverRuleUpdateCommand(waiverRuleId, request));
            return Ok();
        }

        [HttpDelete("{waiverRuleId}")]
        public async Task<ActionResult> Delete(long waiverRuleId)
        {
            await _mediator.Send(new WaiverRuleDeleteCommand(waiverRuleId));
            return Ok();
        }
    }
}
