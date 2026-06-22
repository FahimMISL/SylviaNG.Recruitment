using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IRequisitionApprovalService
    {
        Task<long> CreateAsync(RequisitionApprovalCreateRequest request);
        Task UpdateAsync(long requisitionApprovalId, RequisitionApprovalUpdateRequest request);
        Task DeleteAsync(long requisitionApprovalId);
        Task<List<RequisitionApprovalResponse>> GetAllAsync();
        Task<RequisitionApprovalResponse> GetByIdAsync(long requisitionApprovalId);
        Task<PagedResult<RequisitionApprovalResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
