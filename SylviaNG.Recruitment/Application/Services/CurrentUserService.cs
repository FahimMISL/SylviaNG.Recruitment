using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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
    }
}
