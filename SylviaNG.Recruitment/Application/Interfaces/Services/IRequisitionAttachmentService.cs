using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IRequisitionAttachmentService
    {
        Task<long> CreateAsync(RequisitionAttachmentCreateRequest request);
        Task UpdateAsync(long requisitionAttachmentId, RequisitionAttachmentUpdateRequest request);
        Task DeleteAsync(long requisitionAttachmentId);
        Task<List<RequisitionAttachmentResponse>> GetAllAsync();
        Task<RequisitionAttachmentResponse> GetByIdAsync(long requisitionAttachmentId);
        Task<PagedResult<RequisitionAttachmentResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
