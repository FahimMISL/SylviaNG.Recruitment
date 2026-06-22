using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Queries.ApplicationScreeningResultGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Commands.ApplicationScreeningResultCreate;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Commands.ApplicationScreeningResultDelete;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Commands.ApplicationScreeningResultUpdate;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Models;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Queries.ApplicationScreeningResultGetAll;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Queries.ApplicationScreeningResultGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/application-screening-result")]
    public class ApplicationScreeningResultController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApplicationScreeningResultController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ApplicationScreeningResultResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ApplicationScreeningResultGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ApplicationScreeningResultResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ApplicationScreeningResultGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{applicationScreeningResultId}")]
        public async Task<ActionResult<ApplicationScreeningResultResponse>> GetById(long applicationScreeningResultId)
        {
            var result = await _mediator.Send(new ApplicationScreeningResultGetByIdQuery(applicationScreeningResultId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ApplicationScreeningResultCreateRequest request)
        {
            var id = await _mediator.Send(new ApplicationScreeningResultCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{applicationScreeningResultId}")]
        public async Task<ActionResult> Update(long applicationScreeningResultId, [FromBody] ApplicationScreeningResultUpdateRequest request)
        {
            await _mediator.Send(new ApplicationScreeningResultUpdateCommand(applicationScreeningResultId, request));
            return Ok();
        }

        [HttpDelete("{applicationScreeningResultId}")]
        public async Task<ActionResult> Delete(long applicationScreeningResultId)
        {
            await _mediator.Send(new ApplicationScreeningResultDeleteCommand(applicationScreeningResultId));
            return Ok();
        }
    }
}
