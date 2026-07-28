using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Commands.OfficeNoteGenerate;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Queries.OfficeNoteGetAll;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Queries.OfficeNoteGetById;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Queries.OfficeNoteGetEnclosures;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-12 US-129: office note PDF listing onboarding enclosures on file for a JobApplication.
    [ApiController]
    [Route("recruitment/office-notes")]
    [Authorize(Roles = "Admin,HR")]
    public class OfficeNoteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OfficeNoteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<OfficeNoteResponse>>> GetAll([FromQuery] long? jobApplicationId)
        {
            return Ok(await _mediator.Send(new OfficeNoteGetAllQuery(jobApplicationId)));
        }

        [HttpGet("{officeNoteId}")]
        public async Task<ActionResult<OfficeNoteResponse>> GetById(long officeNoteId)
        {
            return Ok(await _mediator.Send(new OfficeNoteGetByIdQuery(officeNoteId)));
        }

        [HttpGet("enclosures")]
        public async Task<ActionResult<OfficeNoteEnclosuresResponse>> GetEnclosures([FromQuery] long jobApplicationId)
        {
            return Ok(await _mediator.Send(new OfficeNoteGetEnclosuresQuery(jobApplicationId)));
        }

        [HttpPost("generate")]
        public async Task<ActionResult<OfficeNoteResponse>> Generate([FromBody] OfficeNoteGenerateRequest request)
        {
            return Ok(await _mediator.Send(new OfficeNoteGenerateCommand(request)));
        }
    }
}
