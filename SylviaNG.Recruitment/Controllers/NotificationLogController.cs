using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogCreate;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogDelete;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogUpdate;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetAll;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/notification-log")]
    public class NotificationLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationLogResponse>>> GetAll()
        {
            var result = await _mediator.Send(new NotificationLogGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<NotificationLogResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new NotificationLogGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{notificationLogId}")]
        public async Task<ActionResult<NotificationLogResponse>> GetById(long notificationLogId)
        {
            var result = await _mediator.Send(new NotificationLogGetByIdQuery(notificationLogId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] NotificationLogCreateRequest request)
        {
            var id = await _mediator.Send(new NotificationLogCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{notificationLogId}")]
        public async Task<ActionResult> Update(long notificationLogId, [FromBody] NotificationLogUpdateRequest request)
        {
            await _mediator.Send(new NotificationLogUpdateCommand(notificationLogId, request));
            return Ok();
        }

        [HttpDelete("{notificationLogId}")]
        public async Task<ActionResult> Delete(long notificationLogId)
        {
            await _mediator.Send(new NotificationLogDeleteCommand(notificationLogId));
            return Ok();
        }
    }
}
