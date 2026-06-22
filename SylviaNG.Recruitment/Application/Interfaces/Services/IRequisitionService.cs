using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Requisitions.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IRequisitionService
    {
        Task<long> CreateAsync(RequisitionCreateRequest request);
        Task UpdateAsync(long requisitionId, RequisitionUpdateRequest request);
        Task DeleteAsync(long requisitionId);
        Task<List<RequisitionResponse>> GetAllAsync();
        Task<RequisitionResponse> GetByIdAsync(long requisitionId);
        Task<PagedResult<RequisitionResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
