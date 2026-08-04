using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.CandidateNotificationLogMarkAllAsRead;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.CandidateNotificationLogMarkAsRead;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.CandidateNotificationLogGetUnread;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.CandidateNotificationLogGetUnreadCount;

namespace SylviaNG.Recruitment.Controllers
{
    // Candidate-facing counterpart to NotificationLogController (Admin/HR-only). Separate
    // controller rather than mixed authorization on one, since every query here is scoped to the
    // current candidate's own CandidateProfile - see NotificationLogService's *ForCurrentCandidate
    // methods and their IDOR guard on mark-as-read.
    [ApiController]
    [Route("recruitment/candidate-notification-log")]
    [Authorize(Roles = "Candidate")]
    public class CandidateNotificationLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CandidateNotificationLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("unread")]
        public async Task<ActionResult<List<NotificationLogResponse>>> GetUnread()
        {
            return Ok(await _mediator.Send(new CandidateNotificationLogGetUnreadQuery()));
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            return Ok(await _mediator.Send(new CandidateNotificationLogGetUnreadCountQuery()));
        }

        [HttpPost("{notificationLogId}/mark-read")]
        public async Task<ActionResult> MarkAsRead(long notificationLogId)
        {
            await _mediator.Send(new CandidateNotificationLogMarkAsReadCommand(notificationLogId));
            return Ok();
        }

        [HttpPost("mark-all-read")]
        public async Task<ActionResult<int>> MarkAllAsRead()
        {
            return Ok(await _mediator.Send(new CandidateNotificationLogMarkAllAsReadCommand()));
        }
    }
}
