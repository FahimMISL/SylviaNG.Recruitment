using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Commands.AppointmentLetterGenerate;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Queries.AppointmentLetterGetAll;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Queries.AppointmentLetterGetById;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-10 US-083: HR-only, mirrors OfferLetterController's shape.
    [ApiController]
    [Route("recruitment/appointment-letter")]
    [Authorize(Roles = "Admin,HR")]
    public class AppointmentLetterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppointmentLetterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<AppointmentLetterResponse>>> GetAll([FromQuery] long? jobApplicationId)
        {
            return Ok(await _mediator.Send(new AppointmentLetterGetAllQuery(jobApplicationId)));
        }

        [HttpGet("{appointmentLetterId}")]
        public async Task<ActionResult<AppointmentLetterResponse>> GetById(long appointmentLetterId)
        {
            return Ok(await _mediator.Send(new AppointmentLetterGetByIdQuery(appointmentLetterId)));
        }

        [HttpPost("generate")]
        public async Task<ActionResult<AppointmentLetterResponse>> Generate([FromBody] AppointmentLetterGenerateRequest request)
        {
            return Ok(await _mediator.Send(new AppointmentLetterGenerateCommand(request)));
        }
    }
}
