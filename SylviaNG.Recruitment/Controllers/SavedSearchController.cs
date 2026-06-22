using Microsoft.AspNetCore.Authorization;
using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Queries.SavedSearchGetAllPaged;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Commands.SavedSearchCreate;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Commands.SavedSearchDelete;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Commands.SavedSearchUpdate;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Queries.SavedSearchGetAll;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Queries.SavedSearchGetById;

namespace SylviaNG.Recruitment.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    [ApiController]
    [Route("recruitment/saved-search")]
    public class SavedSearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SavedSearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<SavedSearchResponse>>> GetAll()
        {
            var result = await _mediator.Send(new SavedSearchGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<SavedSearchResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new SavedSearchGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{savedSearchId}")]
        public async Task<ActionResult<SavedSearchResponse>> GetById(long savedSearchId)
        {
            var result = await _mediator.Send(new SavedSearchGetByIdQuery(savedSearchId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] SavedSearchCreateRequest request)
        {
            var id = await _mediator.Send(new SavedSearchCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{savedSearchId}")]
        public async Task<ActionResult> Update(long savedSearchId, [FromBody] SavedSearchUpdateRequest request)
        {
            await _mediator.Send(new SavedSearchUpdateCommand(savedSearchId, request));
            return Ok();
        }

        [HttpDelete("{savedSearchId}")]
        public async Task<ActionResult> Delete(long savedSearchId)
        {
            await _mediator.Send(new SavedSearchDeleteCommand(savedSearchId));
            return Ok();
        }
    }
}
