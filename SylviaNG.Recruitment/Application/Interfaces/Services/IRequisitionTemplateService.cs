using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IRequisitionTemplateService
    {
        Task<long> CreateAsync(RequisitionTemplateCreateRequest request);
        Task UpdateAsync(long requisitionTemplateId, RequisitionTemplateUpdateRequest request);
        Task DeleteAsync(long requisitionTemplateId);
        Task<List<RequisitionTemplateResponse>> GetAllAsync();
        Task<RequisitionTemplateResponse> GetByIdAsync(long requisitionTemplateId);
        Task<PagedResult<RequisitionTemplateResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
