using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterCreate;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterDelete;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterUpdate;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetAll;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/shortlist-filter")]
    public class ShortlistFilterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShortlistFilterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ShortlistFilterResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ShortlistFilterGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ShortlistFilterResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ShortlistFilterGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{shortlistFilterId}")]
        public async Task<ActionResult<ShortlistFilterResponse>> GetById(long shortlistFilterId)
        {
            var result = await _mediator.Send(new ShortlistFilterGetByIdQuery(shortlistFilterId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ShortlistFilterCreateRequest request)
        {
            var id = await _mediator.Send(new ShortlistFilterCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{shortlistFilterId}")]
        public async Task<ActionResult> Update(long shortlistFilterId, [FromBody] ShortlistFilterUpdateRequest request)
        {
            await _mediator.Send(new ShortlistFilterUpdateCommand(shortlistFilterId, request));
            return Ok();
        }

        [HttpDelete("{shortlistFilterId}")]
        public async Task<ActionResult> Delete(long shortlistFilterId)
        {
            await _mediator.Send(new ShortlistFilterDeleteCommand(shortlistFilterId));
            return Ok();
        }
    }
}
