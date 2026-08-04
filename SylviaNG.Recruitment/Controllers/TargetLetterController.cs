using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.TargetLetters.Commands.TargetLetterGenerate;
using SylviaNG.Recruitment.Application.Features.TargetLetters.Models;
using SylviaNG.Recruitment.Application.Features.TargetLetters.Queries.TargetLetterGetAll;
using SylviaNG.Recruitment.Application.Features.TargetLetters.Queries.TargetLetterGetById;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-10 US-086: HR-only, mirrors AppointmentLetterController's shape.
    [ApiController]
    [Route("recruitment/target-letter")]
    [Authorize(Roles = "Admin,HR")]
    public class TargetLetterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TargetLetterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<TargetLetterResponse>>> GetAll([FromQuery] long? jobApplicationId)
        {
            return Ok(await _mediator.Send(new TargetLetterGetAllQuery(jobApplicationId)));
        }

        [HttpGet("{targetLetterId}")]
        public async Task<ActionResult<TargetLetterResponse>> GetById(long targetLetterId)
        {
            return Ok(await _mediator.Send(new TargetLetterGetByIdQuery(targetLetterId)));
        }

        [HttpPost("generate")]
        public async Task<ActionResult<TargetLetterResponse>> Generate([FromBody] TargetLetterGenerateRequest request)
        {
            return Ok(await _mediator.Send(new TargetLetterGenerateCommand(request)));
        }
    }
}
