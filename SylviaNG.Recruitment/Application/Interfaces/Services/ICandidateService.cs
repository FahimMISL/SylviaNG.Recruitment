using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateService
    {
        Task<long> CreateAsync(CandidateCreateRequest request);
        Task UpdateAsync(long candidateId, CandidateUpdateRequest request);
        Task DeleteAsync(long candidateId);
        Task<List<CandidateResponse>> GetAllAsync();
        Task<CandidateResponse> GetByIdAsync(long candidateId);
        Task<PagedResult<CandidateResponse>> GetPaginatedAsync(PagedRequest request);
        Task<long> AutoProvisionAsync(CandidateAutoProvisionRequest request);
    }
}
