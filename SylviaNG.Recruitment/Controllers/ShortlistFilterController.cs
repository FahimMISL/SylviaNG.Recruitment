using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterApply;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterCreate;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterDelete;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterUpdate;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetActiveLookup;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetAll;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetById;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterPreview;

namespace SylviaNG.Recruitment.Controllers
{
    [ApiController]
    [Route("recruitment/shortlist-filter")]
    [Authorize(Roles = "Admin,HR")]
    public class ShortlistFilterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShortlistFilterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all shortlist filters with their criteria.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ShortlistFilterResponse>>> GetAll()
        {
            var result = await _mediator.Send(new ShortlistFilterGetAllQuery());
            return Ok(result);
        }

        /// <summary>
        /// Active filters only (Id + Name), for a "pick a saved filter" dropdown.
        /// </summary>
        [HttpGet("lookup")]
        public async Task<ActionResult<List<ShortlistFilterLookupResponse>>> GetActiveLookup()
        {
            var result = await _mediator.Send(new ShortlistFilterGetActiveLookupQuery());
            return Ok(result);
        }

        /// <summary>
        /// Get a shortlist filter by ID, including its ordered criteria.
        /// </summary>
        [HttpGet("{shortlistFilterId}")]
        public async Task<ActionResult<ShortlistFilterResponse>> GetById(long shortlistFilterId)
        {
            var result = await _mediator.Send(new ShortlistFilterGetByIdQuery(shortlistFilterId));
            return Ok(result);
        }

        /// <summary>
        /// Create a new shortlist filter with its criteria.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ShortlistFilterCreateRequest request)
        {
            var id = await _mediator.Send(new ShortlistFilterCreateCommand(request));
            return Ok(id);
        }

        /// <summary>
        /// Update a shortlist filter. The criteria list is a full replacement (add/edit/remove in one save).
        /// </summary>
        [HttpPut("{shortlistFilterId}")]
        public async Task<ActionResult> Update(long shortlistFilterId, [FromBody] ShortlistFilterUpdateRequest request)
        {
            await _mediator.Send(new ShortlistFilterUpdateCommand(shortlistFilterId, request));
            return Ok();
        }

        /// <summary>
        /// Delete a shortlist filter.
        /// </summary>
        [HttpDelete("{shortlistFilterId}")]
        public async Task<ActionResult> Delete(long shortlistFilterId)
        {
            await _mediator.Send(new ShortlistFilterDeleteCommand(shortlistFilterId));
            return Ok();
        }

        /// <summary>
        /// Evaluate a saved or unsaved filter definition against a job posting's applications,
        /// returning how many would pass (US-043 AC5).
        /// </summary>
        [HttpPost("preview")]
        public async Task<ActionResult<ShortlistFilterPreviewResponse>> Preview([FromBody] ShortlistFilterPreviewRequest request)
        {
            var result = await _mediator.Send(new ShortlistFilterPreviewQuery(request));
            return Ok(result);
        }

        /// <summary>
        /// Apply a saved filter to all applications of a vacancy in one action, moving passing
        /// candidates to Shortlisted and returning a processed/shortlisted/failed summary (US-044).
        /// </summary>
        [HttpPost("apply")]
        public async Task<ActionResult<ShortlistFilterApplyResponse>> Apply([FromBody] ShortlistFilterApplyRequest request)
        {
            var result = await _mediator.Send(new ShortlistFilterApplyCommand(request));
            return Ok(result);
        }
    }
}
