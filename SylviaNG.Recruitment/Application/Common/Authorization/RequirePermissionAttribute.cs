using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Application.Common.Authorization
{
    /// <summary>
    /// EP-15/US-112 granular permission gate. Admin/SuperAdmin always pass (same superuser
    /// bypass every other [Authorize(Roles="Admin")] endpoint in the app gets) - everyone else
    /// needs an assigned custom Role that grants (Module, Action) via IPermissionService. Used
    /// instead of [Authorize(Roles=...)] on the new UserAccounts/Roles endpoints so a custom role
    /// (e.g. an "Editor" role granted Admin/Edit but not Admin/Delete) can actually reach them -
    /// a fixed role-name attribute could never express that.
    ///
    /// HR also bypasses, but ONLY for modules in HrBypassModules (JobPostings/Applications/
    /// Interviews/Assessments/Reports - the ones migrated off [Authorize(Roles="Admin,HR")] this
    /// session). Reason: the granular check requires a local UserAccount row with a RoleAssignment,
    /// but plenty of real HR users only exist as Keycloak accounts (created before EP-15's
    /// UserAccounts/invite flow, or added directly in Keycloak) with no such row - those users
    /// would otherwise be locked out of everything HR used to reach via the JWT role claim alone.
    /// This keeps that legacy path working exactly as before. The Admin module (UserAccounts/
    /// Roles screens) is deliberately NOT in the bypass list - HR never had access there and still
    /// doesn't. A user WITH a proper UserAccount+RoleAssignment (via User Accounts screen or a
    /// custom role) still goes through the real per-(module,action) check for everything else -
    /// so a custom role's restrictions, and HR's own seeded RolePermission rows, remain
    /// meaningful for anyone actually invited through the app.
    /// </summary>
    public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private static readonly AccessControlModuleEnum[] HrBypassModules =
        {
            AccessControlModuleEnum.JobPostings,
            AccessControlModuleEnum.Applications,
            AccessControlModuleEnum.Interviews,
            AccessControlModuleEnum.Assessments,
            AccessControlModuleEnum.Reports,
        };

        private readonly AccessControlModuleEnum _module;
        private readonly PermissionActionEnum _action;

        public RequirePermissionAttribute(AccessControlModuleEnum module, PermissionActionEnum action)
        {
            _module = module;
            _action = action;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user.IsInRole(nameof(UserRoleEnum.Admin)) || user.IsInRole(nameof(UserRoleEnum.SuperAdmin)))
                return;

            if (user.IsInRole(nameof(UserRoleEnum.HR)) && HrBypassModules.Contains(_module))
                return;

            var keycloakUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(keycloakUserId))
            {
                context.Result = Forbidden();
                return;
            }

            var permissionService = context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();
            var allowed = await permissionService.HasPermissionAsync(keycloakUserId, _module, _action);

            if (!allowed)
            {
                context.Result = Forbidden();
            }
        }

        private static ObjectResult Forbidden()
        {
            return new ObjectResult(new
            {
                hasError = true,
                decentMessage = "Forbidden. You don't have permission to access this resource.",
                errorDetails = "Access denied. Insufficient permissions.",
                content = (object?)null
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
