using Microsoft.AspNetCore.Authorization;

namespace SylviaNG.Recruitment.Application.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
                options.AddPolicy("AdminOrHR", p => p.RequireRole("Admin", "HR"));
                options.AddPolicy("AllRoles", p => p.RequireRole("Admin", "HR", "Candidate"));
            });

            return services;
        }
    }
}
