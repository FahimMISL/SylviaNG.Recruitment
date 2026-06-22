using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IAssessmentStageService
    {
        Task<long> CreateAsync(AssessmentStageCreateRequest request);
        Task UpdateAsync(long assessmentStageId, AssessmentStageUpdateRequest request);
        Task DeleteAsync(long assessmentStageId);
        Task<List<AssessmentStageResponse>> GetAllAsync();
        Task<AssessmentStageResponse> GetByIdAsync(long assessmentStageId);
        Task<PagedResult<AssessmentStageResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
