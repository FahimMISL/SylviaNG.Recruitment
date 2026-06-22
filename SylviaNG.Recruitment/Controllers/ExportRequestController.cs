using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestCreate;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestDelete;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestUpdate;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestGetAll;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/export-request")]
    public class ExportRequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExportRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ExportRequestResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ExportRequestGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ExportRequestResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ExportRequestGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{exportRequestId}")]
        public async Task<ActionResult<ExportRequestResponse>> GetById(long exportRequestId)
        {
            var result = await _mediator.Send(new ExportRequestGetByIdQuery(exportRequestId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ExportRequestCreateRequest request)
        {
            var id = await _mediator.Send(new ExportRequestCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{exportRequestId}")]
        public async Task<ActionResult> Update(long exportRequestId, [FromBody] ExportRequestUpdateRequest request)
        {
            await _mediator.Send(new ExportRequestUpdateCommand(exportRequestId, request));
            return Ok();
        }

        [HttpDelete("{exportRequestId}")]
        public async Task<ActionResult> Delete(long exportRequestId)
        {
            await _mediator.Send(new ExportRequestDeleteCommand(exportRequestId));
            return Ok();
        }
    }
}
