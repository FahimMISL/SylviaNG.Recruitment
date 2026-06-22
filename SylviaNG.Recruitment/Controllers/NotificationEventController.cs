using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Queries.NotificationEventGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Commands.NotificationEventCreate;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Commands.NotificationEventDelete;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Commands.NotificationEventUpdate;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Models;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Queries.NotificationEventGetAll;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Queries.NotificationEventGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/notification-event")]
    public class NotificationEventController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationEventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationEventResponse>>> GetAll()
        {
            var result = await _mediator.Send(new NotificationEventGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<NotificationEventResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new NotificationEventGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{notificationEventId}")]
        public async Task<ActionResult<NotificationEventResponse>> GetById(long notificationEventId)
        {
            var result = await _mediator.Send(new NotificationEventGetByIdQuery(notificationEventId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] NotificationEventCreateRequest request)
        {
            var id = await _mediator.Send(new NotificationEventCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{notificationEventId}")]
        public async Task<ActionResult> Update(long notificationEventId, [FromBody] NotificationEventUpdateRequest request)
        {
            await _mediator.Send(new NotificationEventUpdateCommand(notificationEventId, request));
            return Ok();
        }

        [HttpDelete("{notificationEventId}")]
        public async Task<ActionResult> Delete(long notificationEventId)
        {
            await _mediator.Send(new NotificationEventDeleteCommand(notificationEventId));
            return Ok();
        }
    }
}
