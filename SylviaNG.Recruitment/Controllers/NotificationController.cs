using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.BulkNotificationSend;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;

namespace SylviaNG.Recruitment.Controllers;

[Authorize(Roles = "Admin,HR")]
[ApiController]
[Route("recruitment/notification")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("bulk-send")]
    public async Task<ActionResult<BulkNotificationResult>> BulkSend([FromBody] BulkNotificationRequest request)
    {
        var result = await _mediator.Send(new BulkNotificationSendCommand(request));
        return Ok(result);
    }
}
