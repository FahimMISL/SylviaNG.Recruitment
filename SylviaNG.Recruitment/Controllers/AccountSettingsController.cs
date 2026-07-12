using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountEmailUpdate;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountPasswordChange;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountPhotoDelete;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountPhotoUpload;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Models;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Queries.AccountSettingsGetMe;

namespace SylviaNG.Recruitment.Controllers
{
    // No class-level [Authorize] - global AuthorizeFilter already requires login. Shared by all
    // 3 roles (Admin/HR/Candidate) - see plan decision on common email/password/photo settings.
    [ApiController]
    [Route("recruitment/account-settings")]
    public class AccountSettingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountSettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        public async Task<ActionResult<AccountSettingsResponse>> GetMe()
        {
            var result = await _mediator.Send(new AccountSettingsGetMeQuery());
            return Ok(result);
        }

        [HttpPut("me/email")]
        public async Task<ActionResult> UpdateEmail([FromBody] AccountEmailUpdateRequest request)
        {
            await _mediator.Send(new AccountEmailUpdateCommand(request));
            return Ok();
        }

        [HttpPut("me/password")]
        public async Task<ActionResult> ChangePassword([FromBody] AccountPasswordChangeRequest request)
        {
            await _mediator.Send(new AccountPasswordChangeCommand(request));
            return Ok();
        }

        [HttpPost("me/photo")]
        public async Task<ActionResult<string>> UploadPhoto([FromForm] IFormFile file)
        {
            var path = await _mediator.Send(new AccountPhotoUploadCommand(file));
            return Ok(path);
        }

        [HttpDelete("me/photo")]
        public async Task<ActionResult> DeletePhoto()
        {
            await _mediator.Send(new AccountPhotoDeleteCommand());
            return Ok();
        }
    }
}
