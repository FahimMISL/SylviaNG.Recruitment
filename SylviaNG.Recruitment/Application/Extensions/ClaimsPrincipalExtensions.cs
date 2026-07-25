using System.Security.Claims;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Keycloak users can carry several realm-role claims (app roles plus
        // defaults like offline_access/uma_authorization), added in whatever
        // order realm_access.roles listed them - FindFirst(ClaimTypes.Role)
        // is not reliable. Mirrors AuthService.ResolveKnownRole's priority scan
        // so a request-time role check agrees with the role returned at login.
        private static readonly UserRoleEnum[] RolePriority = { UserRoleEnum.Admin, UserRoleEnum.HR, UserRoleEnum.Candidate };

        public static UserRoleEnum GetHighestRole(this ClaimsPrincipal user)
        {
            var roleValues = user.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var candidate in RolePriority)
            {
                if (roleValues.Contains(candidate.ToString()))
                    return candidate;
            }

            return UserRoleEnum.Candidate;
        }
    }
}
