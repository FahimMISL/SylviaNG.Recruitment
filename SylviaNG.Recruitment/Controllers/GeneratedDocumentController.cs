using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Queries.GeneratedDocumentGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Commands.GeneratedDocumentCreate;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Commands.GeneratedDocumentDelete;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Commands.GeneratedDocumentUpdate;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Models;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Queries.GeneratedDocumentGetAll;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Queries.GeneratedDocumentGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/generated-document")]
    public class GeneratedDocumentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GeneratedDocumentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneratedDocumentResponse>>> GetAll()
        {
            var result = await _mediator.Send(new GeneratedDocumentGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<GeneratedDocumentResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new GeneratedDocumentGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{generatedDocumentId}")]
        public async Task<ActionResult<GeneratedDocumentResponse>> GetById(long generatedDocumentId)
        {
            var result = await _mediator.Send(new GeneratedDocumentGetByIdQuery(generatedDocumentId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] GeneratedDocumentCreateRequest request)
        {
            var id = await _mediator.Send(new GeneratedDocumentCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{generatedDocumentId}")]
        public async Task<ActionResult> Update(long generatedDocumentId, [FromBody] GeneratedDocumentUpdateRequest request)
        {
            await _mediator.Send(new GeneratedDocumentUpdateCommand(generatedDocumentId, request));
            return Ok();
        }

        [HttpDelete("{generatedDocumentId}")]
        public async Task<ActionResult> Delete(long generatedDocumentId)
        {
            await _mediator.Send(new GeneratedDocumentDeleteCommand(generatedDocumentId));
            return Ok();
        }
    }
}
