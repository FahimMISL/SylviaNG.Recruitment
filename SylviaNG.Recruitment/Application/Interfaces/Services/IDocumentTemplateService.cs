using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IDocumentTemplateService
    {
        Task<long> CreateAsync(DocumentTemplateCreateRequest request);
        Task UpdateAsync(long documentTemplateId, DocumentTemplateUpdateRequest request);
        Task DeleteAsync(long documentTemplateId);
        Task<List<DocumentTemplateResponse>> GetAllAsync();
        Task<DocumentTemplateResponse> GetByIdAsync(long documentTemplateId);
        Task<PagedResult<DocumentTemplateResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
