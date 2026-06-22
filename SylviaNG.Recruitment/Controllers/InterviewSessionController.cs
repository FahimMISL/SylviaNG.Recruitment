using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Queries.InterviewSessionGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Commands.InterviewSessionCreate;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Commands.InterviewSessionDelete;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Commands.InterviewSessionUpdate;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Models;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Queries.InterviewSessionGetAll;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Queries.InterviewSessionGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/interview-session")]
    public class InterviewSessionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InterviewSessionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<InterviewSessionResponse>>> GetAll()
        {
            var result = await _mediator.Send(new InterviewSessionGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<InterviewSessionResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new InterviewSessionGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{interviewSessionId}")]
        public async Task<ActionResult<InterviewSessionResponse>> GetById(long interviewSessionId)
        {
            var result = await _mediator.Send(new InterviewSessionGetByIdQuery(interviewSessionId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] InterviewSessionCreateRequest request)
        {
            var id = await _mediator.Send(new InterviewSessionCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{interviewSessionId}")]
        public async Task<ActionResult> Update(long interviewSessionId, [FromBody] InterviewSessionUpdateRequest request)
        {
            await _mediator.Send(new InterviewSessionUpdateCommand(interviewSessionId, request));
            return Ok();
        }

        [HttpDelete("{interviewSessionId}")]
        public async Task<ActionResult> Delete(long interviewSessionId)
        {
            await _mediator.Send(new InterviewSessionDeleteCommand(interviewSessionId));
            return Ok();
        }
    }
}
