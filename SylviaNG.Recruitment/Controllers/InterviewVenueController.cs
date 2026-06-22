using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueCreate;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueDelete;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueUpdate;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetAll;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/interview-venue")]
    public class InterviewVenueController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InterviewVenueController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<InterviewVenueResponse>>> GetAll()
        {
            var result = await _mediator.Send(new InterviewVenueGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<InterviewVenueResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new InterviewVenueGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{interviewVenueId}")]
        public async Task<ActionResult<InterviewVenueResponse>> GetById(long interviewVenueId)
        {
            var result = await _mediator.Send(new InterviewVenueGetByIdQuery(interviewVenueId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] InterviewVenueCreateRequest request)
        {
            var id = await _mediator.Send(new InterviewVenueCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{interviewVenueId}")]
        public async Task<ActionResult> Update(long interviewVenueId, [FromBody] InterviewVenueUpdateRequest request)
        {
            await _mediator.Send(new InterviewVenueUpdateCommand(interviewVenueId, request));
            return Ok();
        }

        [HttpDelete("{interviewVenueId}")]
        public async Task<ActionResult> Delete(long interviewVenueId)
        {
            await _mediator.Send(new InterviewVenueDeleteCommand(interviewVenueId));
            return Ok();
        }
    }
}
