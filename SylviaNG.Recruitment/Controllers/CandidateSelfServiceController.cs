using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateAutoProvision;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>
    /// Candidate self-service endpoints accessible by any authenticated user.
    /// Separated from CandidateController which requires Admin/HR roles.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("recruitment/candidate-self")]
    public class CandidateSelfServiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CandidateSelfServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Auto-provision a candidate profile on first login after Keycloak registration.
        /// If a candidate with the given email already exists, returns the existing candidateId.
        /// Otherwise creates a new candidate with minimal profile data.
        /// </summary>
        [HttpPost("auto-provision")]
        public async Task<ActionResult<long>> AutoProvision([FromBody] CandidateAutoProvisionRequest request)
        {
            var id = await _mediator.Send(new CandidateAutoProvisionCommand(request));
            return Ok(id);
        }
    }
}
