using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// EP-15/US-112 granular permission engine. Checks a custom Role's permission matrix grant -
    /// callers should short-circuit an Admin/SuperAdmin bypass themselves (see
    /// RequirePermissionAttribute) since system roles aren't rows this service reasons about.
    /// </summary>
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(string keycloakUserId, AccessControlModuleEnum module, PermissionActionEnum action);
    }
}
