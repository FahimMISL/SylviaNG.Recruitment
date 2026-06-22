using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Application.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddKeycloakJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var keycloakSection = configuration.GetSection("Keycloak");
            if (!keycloakSection.Exists())
            {
                throw new ArgumentException("Keycloak configuration section is missing in appsettings.json");
            }

            var authority = keycloakSection.GetValue<string>("Authority") ?? throw new ArgumentNullException("Keycloak:Authority");
            var clientId = keycloakSection.GetValue<string>("ClientId") ?? throw new ArgumentNullException("Keycloak:ClientId");
            var requireHttps = keycloakSection.GetValue<bool>("RequireHttpsMetadata", false);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = clientId;
                options.RequireHttpsMetadata = requireHttps;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = clientId,
                    ValidateIssuer = true,
                    ValidIssuer = authority,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        // Map Keycloak realm_access.roles to standard role claims
                        if (context.Principal?.Identity is ClaimsIdentity identity)
                        {
                            var realmAccess = context.Principal.FindFirst("realm_access");
                            if (realmAccess != null)
                            {
                                try
                                {
                                    var parsed = System.Text.Json.JsonDocument.Parse(realmAccess.Value);
                                    if (parsed.RootElement.TryGetProperty("roles", out var roles))
                                    {
                                        foreach (var role in roles.EnumerateArray())
                                        {
                                            var roleName = role.GetString();
                                            if (!string.IsNullOrEmpty(roleName))
                                                identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                                        }
                                    }
                                }
                                catch { }
                            }

                            // Map preferred_username to Name claim
                            var preferred = context.Principal.FindFirst("preferred_username");
                            if (preferred != null && !identity.HasClaim(c => c.Type == ClaimTypes.Name))
                                identity.AddClaim(new Claim(ClaimTypes.Name, preferred.Value));
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            hasError = true,
                            decentMessage = "Unauthorized. Authentication is required.",
                            errorDetails = context.ErrorDescription ?? "Invalid or missing authentication token.",
                            content = (object?)null
                        };

                        var json = System.Text.Json.JsonSerializer.Serialize(response);
                        return context.Response.WriteAsync(json);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            hasError = true,
                            decentMessage = "Forbidden. You don't have permission to access this resource.",
                            errorDetails = "Access denied. Insufficient permissions.",
                            content = (object?)null
                        };

                        var json = System.Text.Json.JsonSerializer.Serialize(response);
                        return context.Response.WriteAsync(json);
                    }
                };
            });

            return services;
        }
    }
}
