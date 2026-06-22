using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IDocumentAcceptanceService
    {
        Task<long> CreateAsync(DocumentAcceptanceCreateRequest request);
        Task UpdateAsync(long documentAcceptanceId, DocumentAcceptanceUpdateRequest request);
        Task DeleteAsync(long documentAcceptanceId);
        Task<List<DocumentAcceptanceResponse>> GetAllAsync();
        Task<DocumentAcceptanceResponse> GetByIdAsync(long documentAcceptanceId);
        Task<PagedResult<DocumentAcceptanceResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
