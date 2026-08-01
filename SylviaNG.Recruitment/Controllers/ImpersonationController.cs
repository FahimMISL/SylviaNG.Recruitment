using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Impersonation.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-15/US-115: SuperAdmin-only impersonation of an Admin/HR UserAccount (candidate
    // impersonation out of scope - see Doc/features/EP-15-F3-*.md).
    [ApiController]
    [Route("recruitment/impersonation")]
    public class ImpersonationController : ControllerBase
    {
        private readonly IImpersonationService _impersonationService;

        public ImpersonationController(IImpersonationService impersonationService)
        {
            _impersonationService = impersonationService;
        }

        /// <summary>Issues a short-lived (30 min) token carrying the target's identity. The
        /// frontend should swap its active token to this one and keep the original cached to
        /// restore on End.</summary>
        [HttpPost("start")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ImpersonationStartResponse>> Start([FromBody] ImpersonationStartRequest request)
        {
            var result = await _impersonationService.StartAsync(request);
            return Ok(result);
        }

        /// <summary>Ends the session tied to the impersonation token used on THIS request.</summary>
        [HttpPost("end")]
        public async Task<ActionResult> End()
        {
            await _impersonationService.EndCurrentAsync();
            return Ok();
        }
    }
}
