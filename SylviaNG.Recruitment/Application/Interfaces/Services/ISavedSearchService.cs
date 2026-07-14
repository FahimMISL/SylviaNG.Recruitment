using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ISavedSearchService
    {
        Task<long> CreateAsync(SavedSearchCreateRequest request);
        Task UpdateAsync(long savedSearchId, SavedSearchUpdateRequest request);
        Task DeleteAsync(long savedSearchId);

        /// <summary>Current user's own searches plus all shared searches (AC2/AC4).</summary>
        Task<List<SavedSearchLookupResponse>> GetVisibleLookupAsync();
    }
}
