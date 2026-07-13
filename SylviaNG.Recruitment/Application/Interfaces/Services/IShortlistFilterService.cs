using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IShortlistFilterService
    {
        Task<long> CreateAsync(ShortlistFilterCreateRequest request);
        Task UpdateAsync(long shortlistFilterId, ShortlistFilterUpdateRequest request);
        Task DeleteAsync(long shortlistFilterId);
        Task<ShortlistFilterResponse> GetByIdAsync(long shortlistFilterId);
        Task<List<ShortlistFilterResponse>> GetAllAsync();

        /// <summary>Active filters only, for a "pick a saved filter" dropdown.</summary>
        Task<List<ShortlistFilterLookupResponse>> GetActiveLookupAsync();
    }
}
