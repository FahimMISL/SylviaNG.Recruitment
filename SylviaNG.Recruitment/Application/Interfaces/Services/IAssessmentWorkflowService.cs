using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IAssessmentWorkflowService
    {
        Task<long> CreateAsync(AssessmentWorkflowCreateRequest request);
        Task UpdateAsync(long assessmentWorkflowId, AssessmentWorkflowUpdateRequest request);
        Task DeleteAsync(long assessmentWorkflowId);
        Task SetActiveAsync(long assessmentWorkflowId, bool isActive);
        Task<AssessmentWorkflowResponse> GetByIdAsync(long assessmentWorkflowId);
        Task<List<AssessmentWorkflowResponse>> GetAllAsync();

        /// <summary>Active workflows only, for the "assign workflow to job posting" dropdown.</summary>
        Task<List<AssessmentWorkflowLookupResponse>> GetActiveLookupAsync();
    }
}
