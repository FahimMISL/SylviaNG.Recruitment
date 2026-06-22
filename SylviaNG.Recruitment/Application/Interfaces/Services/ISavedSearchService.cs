using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ISavedSearchService
    {
        Task<long> CreateAsync(SavedSearchCreateRequest request);
        Task UpdateAsync(long savedSearchId, SavedSearchUpdateRequest request);
        Task DeleteAsync(long savedSearchId);
        Task<List<SavedSearchResponse>> GetAllAsync();
        Task<SavedSearchResponse> GetByIdAsync(long savedSearchId);
        Task<PagedResult<SavedSearchResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
