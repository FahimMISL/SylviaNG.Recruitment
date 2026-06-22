using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IGeneratedDocumentService
    {
        Task<long> CreateAsync(GeneratedDocumentCreateRequest request);
        Task UpdateAsync(long generatedDocumentId, GeneratedDocumentUpdateRequest request);
        Task DeleteAsync(long generatedDocumentId);
        Task<List<GeneratedDocumentResponse>> GetAllAsync();
        Task<GeneratedDocumentResponse> GetByIdAsync(long generatedDocumentId);
        Task<PagedResult<GeneratedDocumentResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
