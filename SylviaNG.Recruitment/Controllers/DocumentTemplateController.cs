using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateCreate;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateDelete;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateUpdate;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetAll;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetById;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetVersions;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplatePreview;

namespace SylviaNG.Recruitment.Controllers
{
    // EP-10 US-080: nothing here is candidate-facing - every endpoint including GetAll is Admin-only,
    // same convention as NotificationTemplateController.
    [ApiController]
    [Route("recruitment/document-template")]
    [Authorize(Roles = "Admin")]
    public class DocumentTemplateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DocumentTemplateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<DocumentTemplateResponse>>> GetAll()
        {
            return Ok(await _mediator.Send(new DocumentTemplateGetAllQuery()));
        }

        [HttpGet("{documentTemplateId}")]
        public async Task<ActionResult<DocumentTemplateResponse>> GetById(long documentTemplateId)
        {
            return Ok(await _mediator.Send(new DocumentTemplateGetByIdQuery(documentTemplateId)));
        }

        [HttpGet("{documentTemplateId}/versions")]
        public async Task<ActionResult<List<DocumentTemplateVersionResponse>>> GetVersions(long documentTemplateId)
        {
            return Ok(await _mediator.Send(new DocumentTemplateGetVersionsQuery(documentTemplateId)));
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] DocumentTemplateCreateRequest request)
        {
            return Ok(await _mediator.Send(new DocumentTemplateCreateCommand(request)));
        }

        [HttpPut("{documentTemplateId}")]
        public async Task<ActionResult> Update(long documentTemplateId, [FromBody] DocumentTemplateUpdateRequest request)
        {
            await _mediator.Send(new DocumentTemplateUpdateCommand(documentTemplateId, request));
            return Ok();
        }

        [HttpDelete("{documentTemplateId}")]
        public async Task<ActionResult> Delete(long documentTemplateId)
        {
            await _mediator.Send(new DocumentTemplateDeleteCommand(documentTemplateId));
            return Ok();
        }

        [HttpPost("preview")]
        public async Task<ActionResult<DocumentTemplatePreviewResponse>> Preview([FromBody] DocumentTemplatePreviewRequest request)
        {
            return Ok(await _mediator.Send(new DocumentTemplatePreviewQuery(request)));
        }
    }
}
