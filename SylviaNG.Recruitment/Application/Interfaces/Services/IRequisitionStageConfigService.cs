using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IRequisitionStageConfigService
    {
        Task<long> CreateAsync(RequisitionStageConfigCreateRequest request);
        Task UpdateAsync(long requisitionStageConfigId, RequisitionStageConfigUpdateRequest request);
        Task DeleteAsync(long requisitionStageConfigId);
        Task<List<RequisitionStageConfigResponse>> GetAllAsync();
        Task<RequisitionStageConfigResponse> GetByIdAsync(long requisitionStageConfigId);
        Task<PagedResult<RequisitionStageConfigResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
