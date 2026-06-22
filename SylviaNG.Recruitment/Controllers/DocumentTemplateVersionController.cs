using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Queries.DocumentTemplateVersionGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Commands.DocumentTemplateVersionCreate;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Commands.DocumentTemplateVersionDelete;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Commands.DocumentTemplateVersionUpdate;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Queries.DocumentTemplateVersionGetAll;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Queries.DocumentTemplateVersionGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/document-template-version")]
    public class DocumentTemplateVersionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DocumentTemplateVersionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<DocumentTemplateVersionResponse>>> GetAll()
        {
            var result = await _mediator.Send(new DocumentTemplateVersionGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<DocumentTemplateVersionResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new DocumentTemplateVersionGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{documentTemplateVersionId}")]
        public async Task<ActionResult<DocumentTemplateVersionResponse>> GetById(long documentTemplateVersionId)
        {
            var result = await _mediator.Send(new DocumentTemplateVersionGetByIdQuery(documentTemplateVersionId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] DocumentTemplateVersionCreateRequest request)
        {
            var id = await _mediator.Send(new DocumentTemplateVersionCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{documentTemplateVersionId}")]
        public async Task<ActionResult> Update(long documentTemplateVersionId, [FromBody] DocumentTemplateVersionUpdateRequest request)
        {
            await _mediator.Send(new DocumentTemplateVersionUpdateCommand(documentTemplateVersionId, request));
            return Ok();
        }

        [HttpDelete("{documentTemplateVersionId}")]
        public async Task<ActionResult> Delete(long documentTemplateVersionId)
        {
            await _mediator.Send(new DocumentTemplateVersionDeleteCommand(documentTemplateVersionId));
            return Ok();
        }
    }
}
