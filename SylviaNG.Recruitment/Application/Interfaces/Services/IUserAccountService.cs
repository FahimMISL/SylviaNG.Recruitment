using SylviaNG.Recruitment.Application.Features.UserAccounts.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IUserAccountService
    {
        Task<long> CreateAsync(UserAccountCreateRequest request);
        Task UpdateAsync(long userAccountId, UserAccountUpdateRequest request);
        Task SetActiveAsync(long userAccountId, bool isActive);
        Task<UserAccountResponse> GetByIdAsync(long userAccountId);
        Task<List<UserAccountResponse>> GetAllAsync();
    }
}
