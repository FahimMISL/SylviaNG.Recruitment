using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateEducationService
    {
        Task<long> CreateAsync(CandidateEducationCreateRequest request);
        Task UpdateAsync(long candidateEducationId, CandidateEducationUpdateRequest request);
        Task DeleteAsync(long candidateEducationId);
        Task<List<CandidateEducationResponse>> GetAllAsync();
        Task<CandidateEducationResponse> GetByIdAsync(long candidateEducationId);
        Task<PagedResult<CandidateEducationResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
