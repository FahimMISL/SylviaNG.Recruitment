using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SylviaNG.Recruitment.Application.Extensions
{
    public static class AuthenticationExtensions
    {
        public const string KeycloakScheme = "Keycloak";
        public const string LocalScheme = "Local";
        private const string PolicySchemeName = "Bearer";

        /// <summary>
        /// Registers two JWT bearer schemes behind a single "Bearer" policy scheme:
        /// "Keycloak" (real, Authority/JWKS-validated tokens) and "Local" (the
        /// hardcoded-login placeholder issued by AuthService, see EP-15/EP-16 for
        /// when this should be retired). The policy scheme picks a handler by
        /// inspecting the incoming token's issuer, so existing Keycloak-gated
        /// endpoints keep working unchanged for either kind of token.
        /// </summary>
        public static IServiceCollection AddRecruitmentAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var keycloakSection = configuration.GetSection("Keycloak");
            if (!keycloakSection.Exists())
            {
                throw new ArgumentException("Keycloak configuration section is missing in appsettings.json");
            }

            var authority = keycloakSection.GetValue<string>("Authority") ?? throw new ArgumentNullException("Keycloak:Authority");
            var clientId = keycloakSection.GetValue<string>("ClientId") ?? throw new ArgumentNullException("Keycloak:ClientId");
            var requireHttps = keycloakSection.GetValue<bool>("RequireHttpsMetadata", false);

            var localJwtSection = configuration.GetSection("Jwt:Local");
            if (!localJwtSection.Exists())
            {
                throw new ArgumentException("Jwt:Local configuration section is missing in appsettings.json");
            }

            var localSigningKey = localJwtSection.GetValue<string>("SigningKey");
            if (string.IsNullOrWhiteSpace(localSigningKey))
            {
                throw new InvalidOperationException(
                    "Jwt:Local:SigningKey is not configured. Set it via 'dotnet user-secrets set \"Jwt:Local:SigningKey\" \"<value>\"' for local development, or your secret store in other environments.");
            }
            var localIssuer = localJwtSection.GetValue<string>("Issuer") ?? throw new ArgumentNullException("Jwt:Local:Issuer");
            var localAudience = localJwtSection.GetValue<string>("Audience") ?? throw new ArgumentNullException("Jwt:Local:Audience");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = PolicySchemeName;
                options.DefaultChallengeScheme = PolicySchemeName;
            })
            .AddPolicyScheme(PolicySchemeName, PolicySchemeName, options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var authHeader = context.Request.Headers.Authorization.ToString();
                    if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        var token = authHeader["Bearer ".Length..].Trim();
                        var handler = new JwtSecurityTokenHandler();
                        if (handler.CanReadToken(token) && handler.ReadJwtToken(token).Issuer == localIssuer)
                        {
                            return LocalScheme;
                        }
                    }

                    return KeycloakScheme;
                };
            })
            .AddJwtBearer(KeycloakScheme, options =>
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

                options.Events = BuildEvents();
            })
            .AddJwtBearer(LocalScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = localAudience,
                    ValidateIssuer = true,
                    ValidIssuer = localIssuer,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(localSigningKey))
                };

                options.Events = BuildEvents();
            });

            return services;
        }

        private static JwtBearerEvents BuildEvents() => new()
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully");
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
    }
}
