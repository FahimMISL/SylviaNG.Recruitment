using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCreate;
using SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolDelete;
using SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolUpdate;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetAll;
using SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/talent-pool")]
    public class TalentPoolController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TalentPoolController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<TalentPoolResponse>>> GetAll()
        {
            var result = await _mediator.Send(new TalentPoolGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<TalentPoolResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new TalentPoolGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{talentPoolId}")]
        public async Task<ActionResult<TalentPoolResponse>> GetById(long talentPoolId)
        {
            var result = await _mediator.Send(new TalentPoolGetByIdQuery(talentPoolId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] TalentPoolCreateRequest request)
        {
            var id = await _mediator.Send(new TalentPoolCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{talentPoolId}")]
        public async Task<ActionResult> Update(long talentPoolId, [FromBody] TalentPoolUpdateRequest request)
        {
            await _mediator.Send(new TalentPoolUpdateCommand(talentPoolId, request));
            return Ok();
        }

        [HttpDelete("{talentPoolId}")]
        public async Task<ActionResult> Delete(long talentPoolId)
        {
            await _mediator.Send(new TalentPoolDeleteCommand(talentPoolId));
            return Ok();
        }
    }
}
