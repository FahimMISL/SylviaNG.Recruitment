using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IShortlistFilterCriteriaService
    {
        Task<long> CreateAsync(ShortlistFilterCriteriaCreateRequest request);
        Task UpdateAsync(long shortlistFilterCriteriaId, ShortlistFilterCriteriaUpdateRequest request);
        Task DeleteAsync(long shortlistFilterCriteriaId);
        Task<List<ShortlistFilterCriteriaResponse>> GetAllAsync();
        Task<ShortlistFilterCriteriaResponse> GetByIdAsync(long shortlistFilterCriteriaId);
        Task<PagedResult<ShortlistFilterCriteriaResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
