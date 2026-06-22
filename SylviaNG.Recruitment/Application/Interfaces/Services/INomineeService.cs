using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Nominees.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface INomineeService
    {
        Task<long> CreateAsync(NomineeCreateRequest request);
        Task UpdateAsync(long nomineeId, NomineeUpdateRequest request);
        Task DeleteAsync(long nomineeId);
        Task<List<NomineeResponse>> GetAllAsync();
        Task<NomineeResponse> GetByIdAsync(long nomineeId);
        Task<PagedResult<NomineeResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
