using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateDocuments.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateDocumentService
    {
        Task<long> CreateAsync(CandidateDocumentCreateRequest request);
        Task UpdateAsync(long candidateDocumentId, CandidateDocumentUpdateRequest request);
        Task DeleteAsync(long candidateDocumentId);
        Task<List<CandidateDocumentResponse>> GetAllAsync();
        Task<CandidateDocumentResponse> GetByIdAsync(long candidateDocumentId);
        Task<PagedResult<CandidateDocumentResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
