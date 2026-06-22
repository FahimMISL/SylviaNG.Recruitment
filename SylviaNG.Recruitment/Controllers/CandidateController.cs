using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Candidates.Queries.CandidateGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateCreate;
using SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateDelete;
using SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateUpdate;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;
using SylviaNG.Recruitment.Application.Features.Candidates.Queries.CandidateGetAll;
using SylviaNG.Recruitment.Application.Features.Candidates.Queries.CandidateGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/candidate")]
    public class CandidateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CandidateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CandidateResponse>>> GetAll()
        {
            var result = await _mediator.Send(new CandidateGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<CandidateResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new CandidateGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{candidateId}")]
        public async Task<ActionResult<CandidateResponse>> GetById(long candidateId)
        {
            var result = await _mediator.Send(new CandidateGetByIdQuery(candidateId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] CandidateCreateRequest request)
        {
            var id = await _mediator.Send(new CandidateCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{candidateId}")]
        public async Task<ActionResult> Update(long candidateId, [FromBody] CandidateUpdateRequest request)
        {
            await _mediator.Send(new CandidateUpdateCommand(candidateId, request));
            return Ok();
        }

        [HttpDelete("{candidateId}")]
        public async Task<ActionResult> Delete(long candidateId)
        {
            await _mediator.Send(new CandidateDeleteCommand(candidateId));
            return Ok();
        }
    }
}
