using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueCreate;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueSetActiveStatus;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueUpdate;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetActiveLookup;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetAll;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/interview-venue")]
    [Authorize(Roles = "Admin,HR")]
    public class InterviewVenueController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InterviewVenueController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Get all interview venues.</summary>
        [HttpGet]
        public async Task<ActionResult<List<InterviewVenueResponse>>> GetAll()
        {
            var result = await _mediator.Send(new InterviewVenueGetAllQuery());
            return Ok(result);
        }

        /// <summary>Active venues only (Id + Name), for a "pick a venue" dropdown.</summary>
        [HttpGet("lookup")]
        public async Task<ActionResult<List<InterviewVenueLookupResponse>>> GetActiveLookup()
        {
            var result = await _mediator.Send(new InterviewVenueGetActiveLookupQuery());
            return Ok(result);
        }

        /// <summary>Get an interview venue by ID.</summary>
        [HttpGet("{interviewVenueId}")]
        public async Task<ActionResult<InterviewVenueResponse>> GetById(long interviewVenueId)
        {
            var result = await _mediator.Send(new InterviewVenueGetByIdQuery(interviewVenueId));
            return Ok(result);
        }

        /// <summary>Create a new interview venue.</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] InterviewVenueCreateRequest request)
        {
            var id = await _mediator.Send(new InterviewVenueCreateCommand(request));
            return Ok(id);
        }

        /// <summary>Update an interview venue.</summary>
        [HttpPut("{interviewVenueId}")]
        public async Task<ActionResult> Update(long interviewVenueId, [FromBody] InterviewVenueUpdateRequest request)
        {
            await _mediator.Send(new InterviewVenueUpdateCommand(interviewVenueId, request));
            return Ok();
        }

        /// <summary>Activate or deactivate an interview venue.</summary>
        [HttpPatch("{interviewVenueId}/active-status")]
        public async Task<ActionResult> SetActiveStatus(long interviewVenueId, [FromBody] InterviewVenueSetActiveStatusRequest request)
        {
            await _mediator.Send(new InterviewVenueSetActiveStatusCommand(interviewVenueId, request.IsActive));
            return Ok();
        }
    }
}
