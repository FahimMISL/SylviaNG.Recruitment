using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IApplicationScreeningResultService
    {
        Task<long> CreateAsync(ApplicationScreeningResultCreateRequest request);
        Task UpdateAsync(long applicationScreeningResultId, ApplicationScreeningResultUpdateRequest request);
        Task DeleteAsync(long applicationScreeningResultId);
        Task<List<ApplicationScreeningResultResponse>> GetAllAsync();
        Task<ApplicationScreeningResultResponse> GetByIdAsync(long applicationScreeningResultId);
        Task<PagedResult<ApplicationScreeningResultResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
