using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Commands.ExamVenueCreate;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Commands.ExamVenueSetActiveStatus;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Commands.ExamVenueUpdate;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Queries.ExamVenueGetActiveLookup;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Queries.ExamVenueGetAll;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Queries.ExamVenueGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/exam-venue")]
    [Authorize(Roles = "Admin,HR")]
    public class ExamVenueController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamVenueController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Get all exam venues.</summary>
        [HttpGet]
        public async Task<ActionResult<List<ExamVenueResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ExamVenueGetAllQuery());
            return Ok(result);
        }

        /// <summary>Active venues only (Id + Name), for a "pick a venue" dropdown.</summary>
        [HttpGet("lookup")]
        public async Task<ActionResult<List<ExamVenueLookupResponse>>> GetActiveLookup()
        {
            var result = await _mediator.Send(new ExamVenueGetActiveLookupQuery());
            return Ok(result);
        }

        /// <summary>Get an exam venue by ID.</summary>
        [HttpGet("{examVenueId}")]
        public async Task<ActionResult<ExamVenueResponse>> GetById(long examVenueId)
        {
            var result = await _mediator.Send(new ExamVenueGetByIdQuery(examVenueId));
            return Ok(result);
        }

        /// <summary>Create a new exam venue.</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ExamVenueCreateRequest request)
        {
            var id = await _mediator.Send(new ExamVenueCreateCommand(request));
            return Ok(id);
        }

        /// <summary>Update an exam venue.</summary>
        [HttpPut("{examVenueId}")]
        public async Task<ActionResult> Update(long examVenueId, [FromBody] ExamVenueUpdateRequest request)
        {
            await _mediator.Send(new ExamVenueUpdateCommand(examVenueId, request));
            return Ok();
        }

        /// <summary>Activate or deactivate an exam venue.</summary>
        [HttpPatch("{examVenueId}/active-status")]
        public async Task<ActionResult> SetActiveStatus(long examVenueId, [FromBody] ExamVenueSetActiveStatusRequest request)
        {
            await _mediator.Send(new ExamVenueSetActiveStatusCommand(examVenueId, request.IsActive));
            return Ok();
        }
    }
}
