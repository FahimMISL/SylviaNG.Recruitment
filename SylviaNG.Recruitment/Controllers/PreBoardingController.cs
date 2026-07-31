using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingRequestCorrection;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingValidate;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Queries.PreBoardingGetByPoolForHr;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-12 US-096: HR-facing validate/lock + correction-request workflow, separate from the
    // candidate-only PreBoardingCandidateController.
    [ApiController]
    [Route("recruitment/pre-boarding")]
    [Authorize(Roles = "Admin,HR")]
    public class PreBoardingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PreBoardingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-pool/{finalSelectionPoolId}")]
        public async Task<ActionResult<PreBoardingSubmissionResponse>> GetByPool(long finalSelectionPoolId)
        {
            return Ok(await _mediator.Send(new PreBoardingGetByPoolForHrQuery(finalSelectionPoolId)));
        }

        [HttpPost("{preBoardingSubmissionId}/validate")]
        public async Task<ActionResult<PreBoardingSubmissionResponse>> Validate(long preBoardingSubmissionId)
        {
            return Ok(await _mediator.Send(new PreBoardingValidateCommand(preBoardingSubmissionId)));
        }

        [HttpPost("{preBoardingSubmissionId}/request-correction")]
        public async Task<ActionResult<PreBoardingSubmissionResponse>> RequestCorrection(
            long preBoardingSubmissionId, [FromBody] PreBoardingRequestCorrectionRequest request)
        {
            return Ok(await _mediator.Send(new PreBoardingRequestCorrectionCommand(preBoardingSubmissionId, request)));
        }
    }
}
