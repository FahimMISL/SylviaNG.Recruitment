using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IJoiningBookletService
    {
        Task<long> CreateAsync(JoiningBookletCreateRequest request);
        Task UpdateAsync(long joiningBookletId, JoiningBookletUpdateRequest request);
        Task DeleteAsync(long joiningBookletId);
        Task<List<JoiningBookletResponse>> GetAllAsync();
        Task<JoiningBookletResponse> GetByIdAsync(long joiningBookletId);
        Task<PagedResult<JoiningBookletResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
