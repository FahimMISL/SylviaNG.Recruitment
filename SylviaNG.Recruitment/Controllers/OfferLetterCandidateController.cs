using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Commands.OfferLetterAccept;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Commands.OfferLetterDecline;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetAllForCandidate;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetByIdForCandidate;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-10 US-082: candidate self-service - separate from OfferLetterController (Admin-only)
    // per that controller's own comment. Identity is resolved inside the handlers via
    // ICurrentCandidateService, ownership is enforced in OfferLetterService.
    [ApiController]
    [Route("recruitment/me/offer-letter")]
    [Authorize(Roles = "Candidate")]
    public class OfferLetterCandidateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OfferLetterCandidateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<OfferLetterResponse>>> GetAll()
        {
            return Ok(await _mediator.Send(new OfferLetterGetAllForCandidateQuery()));
        }

        [HttpGet("{offerLetterId}")]
        public async Task<ActionResult<OfferLetterResponse>> GetById(long offerLetterId)
        {
            return Ok(await _mediator.Send(new OfferLetterGetByIdForCandidateQuery(offerLetterId)));
        }

        [HttpPost("{offerLetterId}/accept")]
        public async Task<ActionResult<OfferLetterResponse>> Accept(long offerLetterId)
        {
            return Ok(await _mediator.Send(new OfferLetterAcceptCommand(offerLetterId)));
        }

        [HttpPost("{offerLetterId}/decline")]
        public async Task<ActionResult<OfferLetterResponse>> Decline(long offerLetterId, [FromBody] OfferLetterDeclineRequest request)
        {
            return Ok(await _mediator.Send(new OfferLetterDeclineCommand(offerLetterId, request)));
        }
    }
}
