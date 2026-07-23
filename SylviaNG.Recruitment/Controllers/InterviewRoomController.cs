using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Commands.InterviewRoomCreate;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Commands.InterviewRoomSetActiveStatus;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Commands.InterviewRoomUpdate;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Models;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Queries.InterviewRoomGetAllByVenue;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Queries.InterviewRoomGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/interview-venue/{interviewVenueId}/room")]
    [Authorize(Roles = "Admin,HR")]
    public class InterviewRoomController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InterviewRoomController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Get all rooms for an interview venue.</summary>
        [HttpGet]
        public async Task<ActionResult<List<InterviewRoomResponse>>> GetAllByVenue(long interviewVenueId)
        {
            var result = await _mediator.Send(new InterviewRoomGetAllByVenueQuery(interviewVenueId));
            return Ok(result);
        }

        /// <summary>Get a room by ID.</summary>
        [HttpGet("{roomId}")]
        public async Task<ActionResult<InterviewRoomResponse>> GetById(long interviewVenueId, long roomId)
        {
            var result = await _mediator.Send(new InterviewRoomGetByIdQuery(roomId));
            return Ok(result);
        }

        /// <summary>Create a new room under an interview venue.</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create(long interviewVenueId, [FromBody] InterviewRoomCreateRequest request)
        {
            var id = await _mediator.Send(new InterviewRoomCreateCommand(interviewVenueId, request));
            return Ok(id);
        }

        /// <summary>Update a room.</summary>
        [HttpPut("{roomId}")]
        public async Task<ActionResult> Update(long interviewVenueId, long roomId, [FromBody] InterviewRoomUpdateRequest request)
        {
            await _mediator.Send(new InterviewRoomUpdateCommand(roomId, request));
            return Ok();
        }

        /// <summary>Activate or deactivate a room.</summary>
        [HttpPatch("{roomId}/active-status")]
        public async Task<ActionResult> SetActiveStatus(long interviewVenueId, long roomId, [FromBody] InterviewRoomSetActiveStatusRequest request)
        {
            await _mediator.Send(new InterviewRoomSetActiveStatusCommand(roomId, request.IsActive));
            return Ok();
        }
    }
}
