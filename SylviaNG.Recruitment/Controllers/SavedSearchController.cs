using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers
{
    /// <summary>Named, persisted ATS dashboard filter bookmarks, personal or team-shared (US-048).</summary>
    [ApiController]
    [Route("recruitment/saved-search")]
    [Authorize(Roles = "Admin,HR")]
    public class SavedSearchController : ControllerBase
    {
        private readonly ISavedSearchService _savedSearchService;

        public SavedSearchController(ISavedSearchService savedSearchService)
        {
            _savedSearchService = savedSearchService;
        }

        /// <summary>Current user's own saved searches plus every shared one (AC2/AC4) - doubles as the
        /// dropdown source and the Manage Saved Searches dialog source.</summary>
        [HttpGet("lookup")]
        public async Task<ActionResult<List<SavedSearchLookupResponse>>> GetVisibleLookup()
        {
            var result = await _savedSearchService.GetVisibleLookupAsync();
            return Ok(result);
        }

        /// <summary>Save the current filter combo under a name (AC1).</summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] SavedSearchCreateRequest request)
        {
            var id = await _savedSearchService.CreateAsync(request);
            return Ok(id);
        }

        /// <summary>Rename, re-share, or update the stored filter combo (AC3).</summary>
        [HttpPut("{savedSearchId}")]
        public async Task<ActionResult> Update(long savedSearchId, [FromBody] SavedSearchUpdateRequest request)
        {
            await _savedSearchService.UpdateAsync(savedSearchId, request);
            return Ok();
        }

        /// <summary>Delete a saved search (AC3).</summary>
        [HttpDelete("{savedSearchId}")]
        public async Task<ActionResult> Delete(long savedSearchId)
        {
            await _savedSearchService.DeleteAsync(savedSearchId);
            return Ok();
        }
    }
}
