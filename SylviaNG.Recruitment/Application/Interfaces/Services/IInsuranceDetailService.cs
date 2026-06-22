using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IInsuranceDetailService
    {
        Task<long> CreateAsync(InsuranceDetailCreateRequest request);
        Task UpdateAsync(long insuranceDetailId, InsuranceDetailUpdateRequest request);
        Task DeleteAsync(long insuranceDetailId);
        Task<List<InsuranceDetailResponse>> GetAllAsync();
        Task<InsuranceDetailResponse> GetByIdAsync(long insuranceDetailId);
        Task<PagedResult<InsuranceDetailResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
