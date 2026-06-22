using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Queries.InterviewScorecardGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Commands.InterviewScorecardCreate;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Commands.InterviewScorecardDelete;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Commands.InterviewScorecardUpdate;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Queries.InterviewScorecardGetAll;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Queries.InterviewScorecardGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/interview-scorecard")]
    public class InterviewScorecardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InterviewScorecardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<InterviewScorecardResponse>>> GetAll()
        {
            var result = await _mediator.Send(new InterviewScorecardGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<InterviewScorecardResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new InterviewScorecardGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{interviewScorecardId}")]
        public async Task<ActionResult<InterviewScorecardResponse>> GetById(long interviewScorecardId)
        {
            var result = await _mediator.Send(new InterviewScorecardGetByIdQuery(interviewScorecardId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] InterviewScorecardCreateRequest request)
        {
            var id = await _mediator.Send(new InterviewScorecardCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{interviewScorecardId}")]
        public async Task<ActionResult> Update(long interviewScorecardId, [FromBody] InterviewScorecardUpdateRequest request)
        {
            await _mediator.Send(new InterviewScorecardUpdateCommand(interviewScorecardId, request));
            return Ok();
        }

        [HttpDelete("{interviewScorecardId}")]
        public async Task<ActionResult> Delete(long interviewScorecardId)
        {
            await _mediator.Send(new InterviewScorecardDeleteCommand(interviewScorecardId));
            return Ok();
        }
    }
}
