using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IDocumentTemplateVersionService
    {
        Task<long> CreateAsync(DocumentTemplateVersionCreateRequest request);
        Task UpdateAsync(long documentTemplateVersionId, DocumentTemplateVersionUpdateRequest request);
        Task DeleteAsync(long documentTemplateVersionId);
        Task<List<DocumentTemplateVersionResponse>> GetAllAsync();
        Task<DocumentTemplateVersionResponse> GetByIdAsync(long documentTemplateVersionId);
        Task<PagedResult<DocumentTemplateVersionResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
