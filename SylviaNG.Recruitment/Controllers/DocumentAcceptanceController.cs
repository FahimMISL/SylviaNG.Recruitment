using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Queries.DocumentAcceptanceGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Commands.DocumentAcceptanceCreate;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Commands.DocumentAcceptanceDelete;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Commands.DocumentAcceptanceUpdate;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Queries.DocumentAcceptanceGetAll;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Queries.DocumentAcceptanceGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/document-acceptance")]
    public class DocumentAcceptanceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DocumentAcceptanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<DocumentAcceptanceResponse>>> GetAll()
        {
            var result = await _mediator.Send(new DocumentAcceptanceGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<DocumentAcceptanceResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new DocumentAcceptanceGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{documentAcceptanceId}")]
        public async Task<ActionResult<DocumentAcceptanceResponse>> GetById(long documentAcceptanceId)
        {
            var result = await _mediator.Send(new DocumentAcceptanceGetByIdQuery(documentAcceptanceId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] DocumentAcceptanceCreateRequest request)
        {
            var id = await _mediator.Send(new DocumentAcceptanceCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{documentAcceptanceId}")]
        public async Task<ActionResult> Update(long documentAcceptanceId, [FromBody] DocumentAcceptanceUpdateRequest request)
        {
            await _mediator.Send(new DocumentAcceptanceUpdateCommand(documentAcceptanceId, request));
            return Ok();
        }

        [HttpDelete("{documentAcceptanceId}")]
        public async Task<ActionResult> Delete(long documentAcceptanceId)
        {
            await _mediator.Send(new DocumentAcceptanceDeleteCommand(documentAcceptanceId));
            return Ok();
        }
    }
}
