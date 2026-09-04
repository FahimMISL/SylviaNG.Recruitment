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
    // EP-10 US-080: template management (create/update/delete/preview) is Admin-only. GetAll/GetById
    // are also read by HR-facing document-generation forms (e.g. Generate Offer Letter) to populate
    // their template picker, so those two allow HR too - class-level [Authorize] can't express that
    // split (ASP.NET Core ANDs a class-level attribute with any method-level one, it doesn't override),
    // so there's no controller-level [Authorize] here and every action states its own roles.
    [ApiController]
    [Route("recruitment/document-template")]
    public class DocumentTemplateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DocumentTemplateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<List<DocumentTemplateResponse>>> GetAll()
        {
            return Ok(await _mediator.Send(new DocumentTemplateGetAllQuery()));
        }

        [HttpGet("{documentTemplateId}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<DocumentTemplateResponse>> GetById(long documentTemplateId)
        {
            return Ok(await _mediator.Send(new DocumentTemplateGetByIdQuery(documentTemplateId)));
        }

        [HttpGet("{documentTemplateId}/versions")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DocumentTemplateVersionResponse>>> GetVersions(long documentTemplateId)
        {
            return Ok(await _mediator.Send(new DocumentTemplateGetVersionsQuery(documentTemplateId)));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<long>> Create([FromBody] DocumentTemplateCreateRequest request)
        {
            return Ok(await _mediator.Send(new DocumentTemplateCreateCommand(request)));
        }

        [HttpPut("{documentTemplateId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(long documentTemplateId, [FromBody] DocumentTemplateUpdateRequest request)
        {
            await _mediator.Send(new DocumentTemplateUpdateCommand(documentTemplateId, request));
            return Ok();
        }

        [HttpDelete("{documentTemplateId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(long documentTemplateId)
        {
            await _mediator.Send(new DocumentTemplateDeleteCommand(documentTemplateId));
            return Ok();
        }

        // Rendered by every HR-facing "Generate X Letter" form (Medical/Target/Appointment) to
        // fill their live-preview textarea before submit - not template CRUD, so it belongs with
        // GetAll/GetById's Admin,HR split above, not the Admin-only management actions below.
        [HttpPost("preview")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<DocumentTemplatePreviewResponse>> Preview([FromBody] DocumentTemplatePreviewRequest request)
        {
            return Ok(await _mediator.Send(new DocumentTemplatePreviewQuery(request)));
        }
    }
}
