using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IFitmentDataService
    {
        Task<long> CreateAsync(FitmentDataCreateRequest request);
        Task UpdateAsync(long fitmentDataId, FitmentDataUpdateRequest request);
        Task DeleteAsync(long fitmentDataId);
        Task<List<FitmentDataResponse>> GetAllAsync();
        Task<FitmentDataResponse> GetByIdAsync(long fitmentDataId);
        Task<PagedResult<FitmentDataResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
