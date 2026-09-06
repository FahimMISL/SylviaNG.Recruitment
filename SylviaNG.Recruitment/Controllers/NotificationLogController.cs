using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogMarkAllAsRead;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogMarkAsRead;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogRetry;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogExportExcel;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetAll;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetUnread;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetUnreadCount;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-09 US-078/079: log view + in-app bell are both HR-facing, so every endpoint is Admin,HR
    // (unlike NotificationTemplateController, which is Admin-only authoring).
    [ApiController]
    [Route("recruitment/notification-log")]
    [Authorize(Roles = "Admin,HR")]
    public class NotificationLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<NotificationLogResponse>>> GetAll([FromQuery] NotificationLogFilterRequest filter)
        {
            return Ok(await _mediator.Send(new NotificationLogGetAllQuery(filter)));
        }

        [HttpGet("unread")]
        public async Task<ActionResult<List<NotificationLogResponse>>> GetUnread()
        {
            return Ok(await _mediator.Send(new NotificationLogGetUnreadQuery()));
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            return Ok(await _mediator.Send(new NotificationLogGetUnreadCountQuery()));
        }

        [HttpPost("export-excel")]
        public async Task<IActionResult> ExportExcel([FromBody] NotificationLogFilterRequest filter)
        {
            var file = await _mediator.Send(new NotificationLogExportExcelQuery(filter));
            return File(file.Content, file.ContentType, file.FileName);
        }

        [HttpPost("{notificationLogId}/retry")]
        public async Task<ActionResult<NotificationLogResponse>> Retry(long notificationLogId)
        {
            return Ok(await _mediator.Send(new NotificationLogRetryCommand(notificationLogId)));
        }

        [HttpPost("{notificationLogId}/mark-read")]
        public async Task<ActionResult> MarkAsRead(long notificationLogId)
        {
            await _mediator.Send(new NotificationLogMarkAsReadCommand(notificationLogId));
            return Ok();
        }

        [HttpPost("mark-all-read")]
        public async Task<ActionResult<int>> MarkAllAsRead()
        {
            return Ok(await _mediator.Send(new NotificationLogMarkAllAsReadCommand()));
        }
    }
}
