using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingSaveDraft;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingSubmit;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Queries.PreBoardingGetForCandidate;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Queries.PreBoardingIsEligible;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-12 US-095: candidate self-service - identity resolved inside the handlers via
    // ICurrentCandidateService, ownership enforced in PreBoardingService, same shape as
    // OfferLetterCandidateController.
    [ApiController]
    [Route("recruitment/me/pre-boarding")]
    [Authorize(Roles = "Candidate")]
    public class PreBoardingCandidateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PreBoardingCandidateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PreBoardingSubmissionResponse>> Get()
        {
            return Ok(await _mediator.Send(new PreBoardingGetForCandidateQuery()));
        }

        /// <summary>Lightweight, never-throws check for whether the frontend should show the
        /// Pre-Boarding nav item at all (true once the candidate has accepted an offer).</summary>
        [HttpGet("eligible")]
        public async Task<ActionResult<bool>> IsEligible()
        {
            return Ok(await _mediator.Send(new PreBoardingIsEligibleQuery()));
        }

        [HttpPut]
        public async Task<ActionResult<PreBoardingSubmissionResponse>> SaveDraft([FromBody] PreBoardingSaveRequest request)
        {
            return Ok(await _mediator.Send(new PreBoardingSaveDraftCommand(request)));
        }

        [HttpPost("submit")]
        public async Task<ActionResult<PreBoardingSubmissionResponse>> Submit()
        {
            return Ok(await _mediator.Send(new PreBoardingSubmitCommand()));
        }
    }
}
