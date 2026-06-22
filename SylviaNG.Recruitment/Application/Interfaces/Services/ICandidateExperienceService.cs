using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateExperienceService
    {
        Task<long> CreateAsync(CandidateExperienceCreateRequest request);
        Task UpdateAsync(long candidateExperienceId, CandidateExperienceUpdateRequest request);
        Task DeleteAsync(long candidateExperienceId);
        Task<List<CandidateExperienceResponse>> GetAllAsync();
        Task<CandidateExperienceResponse> GetByIdAsync(long candidateExperienceId);
        Task<PagedResult<CandidateExperienceResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
