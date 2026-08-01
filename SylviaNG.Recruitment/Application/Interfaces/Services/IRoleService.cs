using SylviaNG.Recruitment.Application.Features.Roles.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<long> CreateAsync(RoleCreateRequest request);
        Task UpdateAsync(long roleId, RoleUpdateRequest request);
        Task DeleteAsync(long roleId);
        Task<RoleResponse> GetByIdAsync(long roleId);
        Task<List<RoleResponse>> GetAllAsync();
    }
}
