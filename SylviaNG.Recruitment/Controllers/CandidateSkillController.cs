using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Queries.CandidateSkillGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Commands.CandidateSkillCreate;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Commands.CandidateSkillDelete;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Commands.CandidateSkillUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Models;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Queries.CandidateSkillGetAll;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Queries.CandidateSkillGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/candidate-skill")]
    public class CandidateSkillController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CandidateSkillController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CandidateSkillResponse>>> GetAll()
        {
            var result = await _mediator.Send(new CandidateSkillGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<CandidateSkillResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new CandidateSkillGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{candidateSkillId}")]
        public async Task<ActionResult<CandidateSkillResponse>> GetById(long candidateSkillId)
        {
            var result = await _mediator.Send(new CandidateSkillGetByIdQuery(candidateSkillId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] CandidateSkillCreateRequest request)
        {
            var id = await _mediator.Send(new CandidateSkillCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{candidateSkillId}")]
        public async Task<ActionResult> Update(long candidateSkillId, [FromBody] CandidateSkillUpdateRequest request)
        {
            await _mediator.Send(new CandidateSkillUpdateCommand(candidateSkillId, request));
            return Ok();
        }

        [HttpDelete("{candidateSkillId}")]
        public async Task<ActionResult> Delete(long candidateSkillId)
        {
            await _mediator.Send(new CandidateSkillDeleteCommand(candidateSkillId));
            return Ok();
        }
    }
}
