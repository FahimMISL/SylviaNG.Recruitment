using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Queries.InterviewScorecardCriteriaGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Commands.InterviewScorecardCriteriaCreate;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Commands.InterviewScorecardCriteriaDelete;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Commands.InterviewScorecardCriteriaUpdate;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Queries.InterviewScorecardCriteriaGetAll;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Queries.InterviewScorecardCriteriaGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/interview-scorecard-criteria")]
    public class InterviewScorecardCriteriaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InterviewScorecardCriteriaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<InterviewScorecardCriteriaResponse>>> GetAll()
        {
            var result = await _mediator.Send(new InterviewScorecardCriteriaGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<InterviewScorecardCriteriaResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new InterviewScorecardCriteriaGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{interviewScorecardCriteriaId}")]
        public async Task<ActionResult<InterviewScorecardCriteriaResponse>> GetById(long interviewScorecardCriteriaId)
        {
            var result = await _mediator.Send(new InterviewScorecardCriteriaGetByIdQuery(interviewScorecardCriteriaId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] InterviewScorecardCriteriaCreateRequest request)
        {
            var id = await _mediator.Send(new InterviewScorecardCriteriaCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{interviewScorecardCriteriaId}")]
        public async Task<ActionResult> Update(long interviewScorecardCriteriaId, [FromBody] InterviewScorecardCriteriaUpdateRequest request)
        {
            await _mediator.Send(new InterviewScorecardCriteriaUpdateCommand(interviewScorecardCriteriaId, request));
            return Ok();
        }

        [HttpDelete("{interviewScorecardCriteriaId}")]
        public async Task<ActionResult> Delete(long interviewScorecardCriteriaId)
        {
            await _mediator.Send(new InterviewScorecardCriteriaDeleteCommand(interviewScorecardCriteriaId));
            return Ok();
        }
    }
}
