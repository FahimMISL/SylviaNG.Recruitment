using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.UserNotifications.Commands.UserNotificationCreate;
using SylviaNG.Recruitment.Application.Features.UserNotifications.Commands.UserNotificationMarkAllRead;
using SylviaNG.Recruitment.Application.Features.UserNotifications.Commands.UserNotificationMarkRead;
using SylviaNG.Recruitment.Application.Features.UserNotifications.Models;
using SylviaNG.Recruitment.Application.Features.UserNotifications.Queries.UserNotificationGetByUser;
using SylviaNG.Recruitment.Application.Features.UserNotifications.Queries.UserNotificationGetUnreadCount;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers;

[Authorize]
[ApiController]
[Route("recruitment/user-notifications")]
public class UserNotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserNotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] int limit = 20)
    {
        var userId = GetKeycloakUserId();
        if (string.IsNullOrEmpty(userId))
            return Ok(new { hasError = true, decentMessage = "User identity not found." });

        var result = await _mediator.Send(new UserNotificationGetByUserQuery(userId, limit));
        return Ok(result);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = GetKeycloakUserId();
        if (string.IsNullOrEmpty(userId))
            return Ok(new { hasError = true, decentMessage = "User identity not found." });

        var count = await _mediator.Send(new UserNotificationGetUnreadCountQuery(userId));
        return Ok(count);
    }

    [HttpPut("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(long notificationId)
    {
        var userId = GetKeycloakUserId();
        if (string.IsNullOrEmpty(userId))
            return Ok(new { hasError = true, decentMessage = "User identity not found." });

        await _mediator.Send(new UserNotificationMarkReadCommand(notificationId, userId));
        return Ok();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetKeycloakUserId();
        if (string.IsNullOrEmpty(userId))
            return Ok(new { hasError = true, decentMessage = "User identity not found." });

        await _mediator.Send(new UserNotificationMarkAllReadCommand(userId));
        return Ok();
    }

    [HttpDelete("clear-all")]
    public async Task<IActionResult> ClearAll(
        [FromServices] IUserNotificationService notificationService)
    {
        var userId = GetKeycloakUserId();
        if (string.IsNullOrEmpty(userId))
            return Ok(new { hasError = true, decentMessage = "User identity not found." });

        await notificationService.ClearAllAsync(userId);
        return Ok();
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserNotificationCreateRequest request)
    {
        var id = await _mediator.Send(new UserNotificationCreateCommand(request));
        return Ok(id);
    }

    private string? GetKeycloakUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
    }
}
