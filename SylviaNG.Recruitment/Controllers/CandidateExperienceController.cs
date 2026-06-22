using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Queries.CandidateExperienceGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Commands.CandidateExperienceCreate;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Commands.CandidateExperienceDelete;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Commands.CandidateExperienceUpdate;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Queries.CandidateExperienceGetAll;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Queries.CandidateExperienceGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/candidate-experience")]
    public class CandidateExperienceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CandidateExperienceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CandidateExperienceResponse>>> GetAll()
        {
            var result = await _mediator.Send(new CandidateExperienceGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<CandidateExperienceResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new CandidateExperienceGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{candidateExperienceId}")]
        public async Task<ActionResult<CandidateExperienceResponse>> GetById(long candidateExperienceId)
        {
            var result = await _mediator.Send(new CandidateExperienceGetByIdQuery(candidateExperienceId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] CandidateExperienceCreateRequest request)
        {
            var id = await _mediator.Send(new CandidateExperienceCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{candidateExperienceId}")]
        public async Task<ActionResult> Update(long candidateExperienceId, [FromBody] CandidateExperienceUpdateRequest request)
        {
            await _mediator.Send(new CandidateExperienceUpdateCommand(candidateExperienceId, request));
            return Ok();
        }

        [HttpDelete("{candidateExperienceId}")]
        public async Task<ActionResult> Delete(long candidateExperienceId)
        {
            await _mediator.Send(new CandidateExperienceDeleteCommand(candidateExperienceId));
            return Ok();
        }
    }
}
