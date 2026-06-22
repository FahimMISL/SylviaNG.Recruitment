using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IShortlistFilterService
    {
        Task<long> CreateAsync(ShortlistFilterCreateRequest request);
        Task UpdateAsync(long shortlistFilterId, ShortlistFilterUpdateRequest request);
        Task DeleteAsync(long shortlistFilterId);
        Task<List<ShortlistFilterResponse>> GetAllAsync();
        Task<ShortlistFilterResponse> GetByIdAsync(long shortlistFilterId);
        Task<PagedResult<ShortlistFilterResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
