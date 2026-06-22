using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Queries.TalentPoolCandidateGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Commands.TalentPoolCandidateCreate;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Commands.TalentPoolCandidateDelete;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Commands.TalentPoolCandidateUpdate;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Queries.TalentPoolCandidateGetAll;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Queries.TalentPoolCandidateGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/talent-pool-candidate")]
    public class TalentPoolCandidateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TalentPoolCandidateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<TalentPoolCandidateResponse>>> GetAll()
        {
            var result = await _mediator.Send(new TalentPoolCandidateGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<TalentPoolCandidateResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new TalentPoolCandidateGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{talentPoolCandidateId}")]
        public async Task<ActionResult<TalentPoolCandidateResponse>> GetById(long talentPoolCandidateId)
        {
            var result = await _mediator.Send(new TalentPoolCandidateGetByIdQuery(talentPoolCandidateId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] TalentPoolCandidateCreateRequest request)
        {
            var id = await _mediator.Send(new TalentPoolCandidateCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{talentPoolCandidateId}")]
        public async Task<ActionResult> Update(long talentPoolCandidateId, [FromBody] TalentPoolCandidateUpdateRequest request)
        {
            await _mediator.Send(new TalentPoolCandidateUpdateCommand(talentPoolCandidateId, request));
            return Ok();
        }

        [HttpDelete("{talentPoolCandidateId}")]
        public async Task<ActionResult> Delete(long talentPoolCandidateId)
        {
            await _mediator.Send(new TalentPoolCandidateDeleteCommand(talentPoolCandidateId));
            return Ok();
        }
    }
}
