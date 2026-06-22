using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateCreate;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateDelete;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateUpdate;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetAll;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/notification-template")]
    public class NotificationTemplateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationTemplateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationTemplateResponse>>> GetAll()
        {
            var result = await _mediator.Send(new NotificationTemplateGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<NotificationTemplateResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new NotificationTemplateGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{notificationTemplateId}")]
        public async Task<ActionResult<NotificationTemplateResponse>> GetById(long notificationTemplateId)
        {
            var result = await _mediator.Send(new NotificationTemplateGetByIdQuery(notificationTemplateId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] NotificationTemplateCreateRequest request)
        {
            var id = await _mediator.Send(new NotificationTemplateCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{notificationTemplateId}")]
        public async Task<ActionResult> Update(long notificationTemplateId, [FromBody] NotificationTemplateUpdateRequest request)
        {
            await _mediator.Send(new NotificationTemplateUpdateCommand(notificationTemplateId, request));
            return Ok();
        }

        [HttpDelete("{notificationTemplateId}")]
        public async Task<ActionResult> Delete(long notificationTemplateId)
        {
            await _mediator.Send(new NotificationTemplateDeleteCommand(notificationTemplateId));
            return Ok();
        }
    }
}
