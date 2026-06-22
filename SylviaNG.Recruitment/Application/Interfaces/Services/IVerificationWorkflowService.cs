using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IVerificationWorkflowService
    {
        Task<long> CreateAsync(VerificationWorkflowCreateRequest request);
        Task UpdateAsync(long verificationWorkflowId, VerificationWorkflowUpdateRequest request);
        Task DeleteAsync(long verificationWorkflowId);
        Task<List<VerificationWorkflowResponse>> GetAllAsync();
        Task<VerificationWorkflowResponse> GetByIdAsync(long verificationWorkflowId);
        Task<PagedResult<VerificationWorkflowResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
