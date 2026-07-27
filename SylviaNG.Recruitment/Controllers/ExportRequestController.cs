using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestCreate;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestDownload;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestGetPaged;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>EP-13 US-100/104: queued candidate-list export requests, rendered async by
    /// ExportRequestWorker. Shared Admin/HR view, same convention as NotificationLog.</summary>
    [ApiController]
    [Route("recruitment/export-requests")]
    [Authorize(Roles = "Admin,HR")]
    public class ExportRequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExportRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("candidate-list")]
        public async Task<ActionResult<long>> RequestCandidateListExport([FromBody] ExportRequestCreateRequest request)
        {
            var id = await _mediator.Send(new ExportRequestCreateCommand(request));
            return Ok(id);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ExportRequestResponse>>> GetPaged([FromQuery] ExportRequestFilterRequest filter)
        {
            var result = await _mediator.Send(new ExportRequestGetPagedQuery(filter));
            return Ok(result);
        }

        [HttpGet("{exportRequestId:long}/download")]
        public async Task<IActionResult> Download(long exportRequestId)
        {
            var file = await _mediator.Send(new ExportRequestDownloadQuery(exportRequestId));
            return File(file.Content, file.ContentType, file.FileName);
        }
    }
}
