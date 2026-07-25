using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateCreate;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateDelete;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateUpdate;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetAll;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetById;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetVersions;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplatePreview;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-09 US-073: nothing here is candidate-facing (unlike Degree/Country, which candidates need
    // for their own dropdowns) - every endpoint including GetAll is Admin-only.
    [ApiController]
    [Route("recruitment/notification-template")]
    [Authorize(Roles = "Admin")]
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
            return Ok(await _mediator.Send(new NotificationTemplateGetAllQuery()));
        }

        [HttpGet("{notificationTemplateId}")]
        public async Task<ActionResult<NotificationTemplateResponse>> GetById(long notificationTemplateId)
        {
            return Ok(await _mediator.Send(new NotificationTemplateGetByIdQuery(notificationTemplateId)));
        }

        [HttpGet("{notificationTemplateId}/versions")]
        public async Task<ActionResult<List<NotificationTemplateVersionResponse>>> GetVersions(long notificationTemplateId)
        {
            return Ok(await _mediator.Send(new NotificationTemplateGetVersionsQuery(notificationTemplateId)));
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] NotificationTemplateCreateRequest request)
        {
            return Ok(await _mediator.Send(new NotificationTemplateCreateCommand(request)));
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

        [HttpPost("preview")]
        public async Task<ActionResult<NotificationTemplatePreviewResponse>> Preview([FromBody] NotificationTemplatePreviewRequest request)
        {
            return Ok(await _mediator.Send(new NotificationTemplatePreviewQuery(request)));
        }
    }
}
