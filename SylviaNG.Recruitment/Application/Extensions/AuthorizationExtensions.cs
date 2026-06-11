using Microsoft.AspNetCore.Authorization;

namespace SylviaNG.Recruitment.Application.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Add recruitment-specific authorization policies here
            });

            return services;
        }
    }
}
