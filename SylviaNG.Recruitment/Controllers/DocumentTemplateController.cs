using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateCreate;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateDelete;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateUpdate;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetAll;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
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
        public async Task<ActionResult<List<DocumentTemplateResponse>>> GetAll()
        {
            var result = await _mediator.Send(new DocumentTemplateGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<DocumentTemplateResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new DocumentTemplateGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{documentTemplateId}")]
        public async Task<ActionResult<DocumentTemplateResponse>> GetById(long documentTemplateId)
        {
            var result = await _mediator.Send(new DocumentTemplateGetByIdQuery(documentTemplateId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] DocumentTemplateCreateRequest request)
        {
            var id = await _mediator.Send(new DocumentTemplateCreateCommand(request));
            return Ok(id);
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
    }
}
