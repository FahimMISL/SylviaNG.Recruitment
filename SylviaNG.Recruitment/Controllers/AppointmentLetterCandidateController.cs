using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Queries.AppointmentLetterGetAllForCandidate;
using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Queries.AppointmentLetterGetByIdForCandidate;

namespace SylviaNG.Recruitment.Controllers
{
    // Candidate self-service - separate from AppointmentLetterController (Admin/HR-only) per that
    // controller's own convention, mirrors OfferLetterCandidateController's shape. Identity is
    // resolved inside the handlers via ICurrentCandidateService, ownership is enforced in
    // AppointmentLetterService.GetOwnedAppointmentLetterAsync.
    [ApiController]
    [Route("recruitment/me/appointment-letter")]
    [Authorize(Roles = "Candidate")]
    public class AppointmentLetterCandidateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppointmentLetterCandidateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<AppointmentLetterResponse>>> GetAll()
        {
            return Ok(await _mediator.Send(new AppointmentLetterGetAllForCandidateQuery()));
        }

        [HttpGet("{appointmentLetterId}")]
        public async Task<ActionResult<AppointmentLetterResponse>> GetById(long appointmentLetterId)
        {
            return Ok(await _mediator.Send(new AppointmentLetterGetByIdForCandidateQuery(appointmentLetterId)));
        }
    }
}
