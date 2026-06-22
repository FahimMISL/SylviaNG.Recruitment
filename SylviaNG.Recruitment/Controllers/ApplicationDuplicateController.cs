using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Queries.ApplicationDuplicateGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Commands.ApplicationDuplicateCreate;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Commands.ApplicationDuplicateDelete;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Commands.ApplicationDuplicateUpdate;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Queries.ApplicationDuplicateGetAll;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Queries.ApplicationDuplicateGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/application-duplicate")]
    public class ApplicationDuplicateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApplicationDuplicateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ApplicationDuplicateResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ApplicationDuplicateGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ApplicationDuplicateResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ApplicationDuplicateGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{applicationDuplicateId}")]
        public async Task<ActionResult<ApplicationDuplicateResponse>> GetById(long applicationDuplicateId)
        {
            var result = await _mediator.Send(new ApplicationDuplicateGetByIdQuery(applicationDuplicateId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ApplicationDuplicateCreateRequest request)
        {
            var id = await _mediator.Send(new ApplicationDuplicateCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{applicationDuplicateId}")]
        public async Task<ActionResult> Update(long applicationDuplicateId, [FromBody] ApplicationDuplicateUpdateRequest request)
        {
            await _mediator.Send(new ApplicationDuplicateUpdateCommand(applicationDuplicateId, request));
            return Ok();
        }

        [HttpDelete("{applicationDuplicateId}")]
        public async Task<ActionResult> Delete(long applicationDuplicateId)
        {
            await _mediator.Send(new ApplicationDuplicateDeleteCommand(applicationDuplicateId));
            return Ok();
        }
    }
}
