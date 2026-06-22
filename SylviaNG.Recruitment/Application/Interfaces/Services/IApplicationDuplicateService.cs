using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IApplicationDuplicateService
    {
        Task<long> CreateAsync(ApplicationDuplicateCreateRequest request);
        Task UpdateAsync(long applicationDuplicateId, ApplicationDuplicateUpdateRequest request);
        Task DeleteAsync(long applicationDuplicateId);
        Task<List<ApplicationDuplicateResponse>> GetAllAsync();
        Task<ApplicationDuplicateResponse> GetByIdAsync(long applicationDuplicateId);
        Task<PagedResult<ApplicationDuplicateResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
