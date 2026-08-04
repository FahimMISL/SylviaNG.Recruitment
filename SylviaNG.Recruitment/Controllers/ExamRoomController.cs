using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Commands.ExamRoomCreate;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Commands.ExamRoomSetActiveStatus;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Commands.ExamRoomUpdate;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Models;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Queries.ExamRoomGetAllByVenue;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Queries.ExamRoomGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/exam-venue/{examVenueId}/room")]
    [Authorize(Roles = "Admin,HR")]
    public class ExamRoomController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamRoomController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Get all rooms for an exam venue.</summary>
        [HttpGet]
        public async Task<ActionResult<List<ExamRoomResponse>>> GetAllByVenue(long examVenueId)
        {
            var result = await _mediator.Send(new ExamRoomGetAllByVenueQuery(examVenueId));
            return Ok(result);
        }

        /// <summary>Get a room by ID.</summary>
        [HttpGet("{roomId}")]
        public async Task<ActionResult<ExamRoomResponse>> GetById(long examVenueId, long roomId)
        {
            var result = await _mediator.Send(new ExamRoomGetByIdQuery(roomId));
            return Ok(result);
        }

        /// <summary>Create a new room under an exam venue.</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create(long examVenueId, [FromBody] ExamRoomCreateRequest request)
        {
            var id = await _mediator.Send(new ExamRoomCreateCommand(examVenueId, request));
            return Ok(id);
        }

        /// <summary>Update a room.</summary>
        [HttpPut("{roomId}")]
        public async Task<ActionResult> Update(long examVenueId, long roomId, [FromBody] ExamRoomUpdateRequest request)
        {
            await _mediator.Send(new ExamRoomUpdateCommand(roomId, request));
            return Ok();
        }

        /// <summary>Activate or deactivate a room.</summary>
        [HttpPatch("{roomId}/active-status")]
        public async Task<ActionResult> SetActiveStatus(long examVenueId, long roomId, [FromBody] ExamRoomSetActiveStatusRequest request)
        {
            await _mediator.Send(new ExamRoomSetActiveStatusCommand(roomId, request.IsActive));
            return Ok();
        }
    }
}
