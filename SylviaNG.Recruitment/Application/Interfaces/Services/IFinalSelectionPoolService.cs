using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IFinalSelectionPoolService
    {
        Task<long> CreateAsync(FinalSelectionPoolCreateRequest request);
        Task UpdateAsync(long finalSelectionPoolId, FinalSelectionPoolUpdateRequest request);
        Task DeleteAsync(long finalSelectionPoolId);
        Task<List<FinalSelectionPoolResponse>> GetAllAsync();
        Task<FinalSelectionPoolResponse> GetByIdAsync(long finalSelectionPoolId);
        Task<PagedResult<FinalSelectionPoolResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
