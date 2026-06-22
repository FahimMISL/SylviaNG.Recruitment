using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Roles.Models;
using SylviaNG.Recruitment.Application.Features.Users.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<long> CreateAsync(UserCreateRequest request);
        Task UpdateAsync(long userId, UserUpdateRequest request);
        Task DeleteAsync(long userId);
        Task<UserResponse> GetByIdAsync(long userId);
        Task<List<UserResponse>> GetAllAsync();
        Task AssignRoleAsync(long userId, UserRoleAssignRequest request);
        Task RemoveRoleAsync(long userId, long roleId);
        Task<PagedResult<UserResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
