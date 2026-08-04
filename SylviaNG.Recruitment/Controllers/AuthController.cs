using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.ForgotPassword;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.Login;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.Refresh;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.Register;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.ResendOtp;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.ResetPassword;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.VerifyOtp;
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

        /// <summary>
        /// EP-09 Feature 2: completes a candidate login that Login returned with RequiresOtp=true.
        /// AllowAnonymous - no token exists yet at this point, only the opaque ChallengeId proves
        /// the caller already passed the Keycloak credential check.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<ActionResult<LoginResponse>> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var result = await _mediator.Send(new VerifyOtpCommand(request));
            return Ok(result);
        }

        /// <summary>
        /// Re-sends a fresh OTP code for a still-open candidate login challenge.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("resend-otp")]
        public async Task<ActionResult<ResendOtpResponse>> ResendOtp([FromBody] ResendOtpRequest request)
        {
            var result = await _mediator.Send(new ResendOtpCommand(request));
            return Ok(result);
        }

        /// <summary>
        /// Starts a forgot-password challenge for any role. Always returns 200 with a ChallengeId,
        /// even for a username that doesn't resolve to a Keycloak user, so this endpoint can't be
        /// used to enumerate accounts - a real email only goes out when the username is real.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _mediator.Send(new ForgotPasswordCommand(request));
            return Ok(result);
        }

        /// <summary>
        /// Completes a forgot-password challenge: verifies the OTP that ForgotPassword emailed and
        /// sets the new password directly via Keycloak - no old password required.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _mediator.Send(new ResetPasswordCommand(request));
            return Ok();
        }
    }
}
