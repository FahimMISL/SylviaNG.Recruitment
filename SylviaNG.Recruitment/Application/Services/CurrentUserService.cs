using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccountRepository _userAccountRepository;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUserAccountRepository userAccountRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userAccountRepository = userAccountRepository;
        }

        public string? GetCurrentUserName()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst("preferred_username")?.Value ?? user?.FindFirst(ClaimTypes.Name)?.Value;
        }

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }

        public string? GetCurrentUserEmail()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Email)?.Value ?? user?.FindFirst("email")?.Value;
        }

        public async Task<long?> GetCurrentUserCompanyIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var keycloakUserId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(keycloakUserId))
                return null;

            var account = await _userAccountRepository.GetByKeycloakUserIdWithRolesAsync(keycloakUserId);
            return account?.CompanyId;
        }
    }
}
