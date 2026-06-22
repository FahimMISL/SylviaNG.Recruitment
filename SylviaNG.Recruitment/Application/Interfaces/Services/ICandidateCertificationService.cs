using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateCertificationService
    {
        Task<long> CreateAsync(CandidateCertificationCreateRequest request);
        Task UpdateAsync(long candidateCertificationId, CandidateCertificationUpdateRequest request);
        Task DeleteAsync(long candidateCertificationId);
        Task<List<CandidateCertificationResponse>> GetAllAsync();
        Task<CandidateCertificationResponse> GetByIdAsync(long candidateCertificationId);
        Task<PagedResult<CandidateCertificationResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
