using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Commands.MedicalLetterGenerate;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Models;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Queries.MedicalLetterGetAll;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Queries.MedicalLetterGetById;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-10 US-086: HR-only, mirrors AppointmentLetterController's shape.
    [ApiController]
    [Route("recruitment/medical-letter")]
    [Authorize(Roles = "Admin")]
    public class MedicalLetterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MedicalLetterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<MedicalLetterResponse>>> GetAll([FromQuery] long? jobApplicationId)
        {
            return Ok(await _mediator.Send(new MedicalLetterGetAllQuery(jobApplicationId)));
        }

        [HttpGet("{medicalLetterId}")]
        public async Task<ActionResult<MedicalLetterResponse>> GetById(long medicalLetterId)
        {
            return Ok(await _mediator.Send(new MedicalLetterGetByIdQuery(medicalLetterId)));
        }

        [HttpPost("generate")]
        public async Task<ActionResult<MedicalLetterResponse>> Generate([FromBody] MedicalLetterGenerateRequest request)
        {
            return Ok(await _mediator.Send(new MedicalLetterGenerateCommand(request)));
        }
    }
}
