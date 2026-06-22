using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IAssessmentWorkflowService
    {
        Task<long> CreateAsync(AssessmentWorkflowCreateRequest request);
        Task UpdateAsync(long assessmentWorkflowId, AssessmentWorkflowUpdateRequest request);
        Task DeleteAsync(long assessmentWorkflowId);
        Task<List<AssessmentWorkflowResponse>> GetAllAsync();
        Task<AssessmentWorkflowResponse> GetByIdAsync(long assessmentWorkflowId);
        Task<PagedResult<AssessmentWorkflowResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
