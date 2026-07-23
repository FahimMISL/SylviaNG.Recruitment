using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.Login;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.Refresh;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.Register;
using SylviaNG.Recruitment.Application.Features.Auth.Models;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Log in via Keycloak (ROPC proxied server-side) and receive the Keycloak access
        /// token. Falls back to the offline hardcoded accounts when Keycloak is unreachable.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await _mediator.Send(new LoginCommand(request));
            return Ok(result);
        }

        /// <summary>
        /// Exchanges a still-valid refresh token for a new access token, so the frontend can
        /// keep a session alive past Keycloak's short access-token lifespan without forcing a
        /// re-login. AllowAnonymous because the caller's access token is, by definition,
        /// already expired by the time this is hit - only the refresh token proves identity.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponse>> Refresh([FromBody] RefreshRequest request)
        {
            var result = await _mediator.Send(new RefreshCommand(request));
            return Ok(result);
        }

        /// <summary>
        /// Self-register as an external candidate (US-001). Creates a Candidate-role user
        /// in Keycloak; email verification is required before first login when enabled.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            var result = await _mediator.Send(new RegisterCommand(request));
            return Ok(result);
        }
    }
}
