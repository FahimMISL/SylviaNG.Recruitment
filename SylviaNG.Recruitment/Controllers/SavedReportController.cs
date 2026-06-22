using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.SavedReports.Queries.SavedReportGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.SavedReports.Commands.SavedReportCreate;
using SylviaNG.Recruitment.Application.Features.SavedReports.Commands.SavedReportDelete;
using SylviaNG.Recruitment.Application.Features.SavedReports.Commands.SavedReportUpdate;
using SylviaNG.Recruitment.Application.Features.SavedReports.Models;
using SylviaNG.Recruitment.Application.Features.SavedReports.Queries.SavedReportGetAll;
using SylviaNG.Recruitment.Application.Features.SavedReports.Queries.SavedReportGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/saved-report")]
    public class SavedReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SavedReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<SavedReportResponse>>> GetAll()
        {
            var result = await _mediator.Send(new SavedReportGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<SavedReportResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new SavedReportGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{savedReportId}")]
        public async Task<ActionResult<SavedReportResponse>> GetById(long savedReportId)
        {
            var result = await _mediator.Send(new SavedReportGetByIdQuery(savedReportId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] SavedReportCreateRequest request)
        {
            var id = await _mediator.Send(new SavedReportCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{savedReportId}")]
        public async Task<ActionResult> Update(long savedReportId, [FromBody] SavedReportUpdateRequest request)
        {
            await _mediator.Send(new SavedReportUpdateCommand(savedReportId, request));
            return Ok();
        }

        [HttpDelete("{savedReportId}")]
        public async Task<ActionResult> Delete(long savedReportId)
        {
            await _mediator.Send(new SavedReportDeleteCommand(savedReportId));
            return Ok();
        }
    }
}
