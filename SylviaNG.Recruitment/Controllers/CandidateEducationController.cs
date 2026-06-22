using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Queries.CandidateEducationGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Commands.CandidateEducationCreate;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Commands.CandidateEducationDelete;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Commands.CandidateEducationUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Models;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Queries.CandidateEducationGetAll;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Queries.CandidateEducationGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/candidate-education")]
    public class CandidateEducationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CandidateEducationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CandidateEducationResponse>>> GetAll()
        {
            var result = await _mediator.Send(new CandidateEducationGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<CandidateEducationResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new CandidateEducationGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{candidateEducationId}")]
        public async Task<ActionResult<CandidateEducationResponse>> GetById(long candidateEducationId)
        {
            var result = await _mediator.Send(new CandidateEducationGetByIdQuery(candidateEducationId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] CandidateEducationCreateRequest request)
        {
            var id = await _mediator.Send(new CandidateEducationCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{candidateEducationId}")]
        public async Task<ActionResult> Update(long candidateEducationId, [FromBody] CandidateEducationUpdateRequest request)
        {
            await _mediator.Send(new CandidateEducationUpdateCommand(candidateEducationId, request));
            return Ok();
        }

        [HttpDelete("{candidateEducationId}")]
        public async Task<ActionResult> Delete(long candidateEducationId)
        {
            await _mediator.Send(new CandidateEducationDeleteCommand(candidateEducationId));
            return Ok();
        }
    }
}
