using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUserAccountRepository _userAccountRepository;

        public PermissionService(IUserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        public async Task<bool> HasPermissionAsync(string keycloakUserId, AccessControlModuleEnum module, PermissionActionEnum action)
        {
            var account = await _userAccountRepository.GetByKeycloakUserIdWithRolesAsync(keycloakUserId);
            if (account is null || !account.IsActive)
                return false;

            return account.RoleAssignments
                .SelectMany(a => a.Role.Permissions)
                .Any(p => p.Module == module && p.Action == action);
        }
    }
}
