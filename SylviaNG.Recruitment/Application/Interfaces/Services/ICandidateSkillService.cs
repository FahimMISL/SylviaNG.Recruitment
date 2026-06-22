using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateSkillService
    {
        Task<long> CreateAsync(CandidateSkillCreateRequest request);
        Task UpdateAsync(long candidateSkillId, CandidateSkillUpdateRequest request);
        Task DeleteAsync(long candidateSkillId);
        Task<List<CandidateSkillResponse>> GetAllAsync();
        Task<CandidateSkillResponse> GetByIdAsync(long candidateSkillId);
        Task<PagedResult<CandidateSkillResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
