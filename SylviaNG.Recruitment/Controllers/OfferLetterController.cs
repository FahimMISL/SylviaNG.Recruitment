using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Commands.OfferLetterGenerate;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetAll;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetById;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-10 US-081: HR-only for this feature - candidate accept/decline (F2, US-082/083) will add
    // its own candidate-facing endpoints separately, not here.
    [ApiController]
    [Route("recruitment/offer-letter")]
    [Authorize(Roles = "Admin")]
    public class OfferLetterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OfferLetterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<OfferLetterResponse>>> GetAll([FromQuery] long? jobApplicationId)
        {
            return Ok(await _mediator.Send(new OfferLetterGetAllQuery(jobApplicationId)));
        }

        [HttpGet("{offerLetterId}")]
        public async Task<ActionResult<OfferLetterResponse>> GetById(long offerLetterId)
        {
            return Ok(await _mediator.Send(new OfferLetterGetByIdQuery(offerLetterId)));
        }

        [HttpPost("generate")]
        public async Task<ActionResult<OfferLetterResponse>> Generate([FromBody] OfferLetterGenerateRequest request)
        {
            return Ok(await _mediator.Send(new OfferLetterGenerateCommand(request)));
        }
    }
}
