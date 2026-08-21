using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Middlewares
{
    /// <summary>
    /// Resolves the current request's CompanyId once and caches it on HttpContext.Items, for
    /// ApplicationDBContext.CurrentCompanyId (the ICompanyScoped global query filter) to read
    /// synchronously during query translation. Looked up from the local UserAccounts table by
    /// the caller's Keycloak sub claim - not a JWT claim like CurrentTenantId uses, since no
    /// Keycloak realm attribute/mapper for company exists (and adding one is out of scope here).
    /// SuperAdmin is left unset (null => unrestricted, global across every company). Runs after
    /// UseAuthentication/ImpersonationMiddleware so claims reflect the final resolved identity.
    /// </summary>
    public class CompanyScopeMiddleware
    {
        private readonly RequestDelegate _next;

        public CompanyScopeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDBContext dbContext)
        {
            var user = context.User;
            if (user?.Identity?.IsAuthenticated == true && !user.IsInRole(nameof(UserRoleEnum.SuperAdmin)))
            {
                var keycloakUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;
                if (!string.IsNullOrEmpty(keycloakUserId))
                {
                    // IgnoreQueryFilters: this lookup bootstraps CurrentCompanyId itself, so the
                    // ICompanyScoped filter (which reads CurrentCompanyId) can't apply here yet -
                    // without this the query would filter against its own not-yet-resolved value.
                    var companyId = await dbContext.UserAccounts
                        .IgnoreQueryFilters()
                        .Where(u => u.KeycloakUserId == keycloakUserId)
                        .Select(u => u.CompanyId)
                        .FirstOrDefaultAsync();

                    context.Items[ApplicationDBContext.CompanyScopeItemKey] = companyId;
                }
            }

            await _next(context);
        }
    }
}
