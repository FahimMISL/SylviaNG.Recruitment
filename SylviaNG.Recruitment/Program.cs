using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using SylviaNG.Recruitment.Application.Extensions;
using SylviaNG.Recruitment.Infrastructure.Extensions;
using SylviaNG.Recruitment.Middlewares;
using SylviaNG.Recruitment.SharedKernel.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddGrpcServices(builder.Configuration);

// Whitelisted to the app's own frontend(s) rather than AllowAnyOrigin - a wildcard origin let
// any website's JS call anonymous endpoints (login, register, forgot-password, career-portal
// apply) directly from a victim's browser. Portal:FrontendBaseUrl is the same config value the
// rest of the app already treats as "the frontend" (portal deep-links, SSLCommerz return URL);
// CORS:AdditionalOrigins covers any other deployed frontend (e.g. a separate demo host) without
// another code change.
var corsOrigins = new[] { builder.Configuration["Portal:FrontendBaseUrl"] }
    .Concat(builder.Configuration.GetSection("Cors:AdditionalOrigins").Get<string[]>() ?? Array.Empty<string>())
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Distinct()
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(corsOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Password login/register/OTP/reset had zero application-level brute-force protection - only
// Keycloak's own (unverifiable from this repo) throttling stood between an attacker and
// unlimited credential/OTP guesses. Partitioned per client IP so one attacker can't exhaust a
// shared bucket and lock out everyone else; rejects outright past the limit rather than queuing.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});

builder.Services.AddControllers();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SylviaNG Recruitment API",
        Version = "v1",
        Description = "Recruitment Management API with Keycloak Authentication"
    });

    // Add JWT Bearer Authentication
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(document =>
    new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = new List<string>()
    });
});


// Add authentication: real Keycloak-issued tokens + the temporary hardcoded-login tokens
builder.Services.AddRecruitmentAuthentication(builder.Configuration);

builder.Services.AddAuthorizationPolicies();

// Add global authorization policy - all endpoints require authentication by default
builder.Services.AddControllers(options =>
{
    // Global authorization filter - all endpoints require authentication
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new LocalDateTimeJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new NullableLocalDateTimeJsonConverter());
    });




var app = builder.Build();

// Fills the "who creates the first SuperAdmin?" gap - see SuperAdminBootstrapService for the
// full explanation. No-op once any SuperAdmin exists, so this stays safe to run on every boot.
await using (var bootstrapScope = app.Services.CreateAsyncScope())
{
    var bootstrapService = bootstrapScope.ServiceProvider.GetRequiredService<SylviaNG.Recruitment.Application.Interfaces.Services.ISuperAdminBootstrapService>();
    await bootstrapService.RunAsync();
}

// Fail fast rather than silently run insecurely: an HTTP Keycloak authority lets a
// network-position attacker tamper with JWKS/token-endpoint traffic, and the MinIO defaults
// are fixed, publicly-known credentials (only meant for the local docker container - see
// appsettings.json's Minio section comment). Both are fine in Development (LAN-only Keycloak,
// local docker MinIO) but must never reach a real deployment unconfigured.
if (!app.Environment.IsDevelopment())
{
    var keycloakAuthority = app.Configuration["Keycloak:Authority"];
    if (keycloakAuthority?.StartsWith("http://", StringComparison.OrdinalIgnoreCase) == true)
    {
        throw new InvalidOperationException(
            $"Keycloak:Authority ('{keycloakAuthority}') must use HTTPS outside Development.");
    }

    if (string.Equals(app.Configuration["FileStorage:Provider"], "Minio", StringComparison.OrdinalIgnoreCase))
    {
        var minioAccessKey = app.Configuration["Minio:AccessKey"];
        var minioSecretKey = app.Configuration["Minio:SecretKey"];
        if (minioAccessKey == "minioadmin" || minioSecretKey == "minioadmin123")
        {
            throw new InvalidOperationException(
                "Minio:AccessKey/Minio:SecretKey are still the default dev-container credentials - set real ones outside Development.");
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";

    // FilesController.Download is deliberately [AllowAnonymous] and embeddable (see its own doc
    // comment) - candidate/HR document viewers (offer/medical/target letters, appointment
    // letters) render its response inside an <iframe>, which a blanket DENY here silently blocks
    // (net::ERR_BLOCKED_BY_RESPONSE) with no server-side error to point at. Every other response
    // keeps the clickjacking protection.
    if (!context.Request.Path.StartsWithSegments("/recruitment/files/download"))
        context.Response.Headers["X-Frame-Options"] = "DENY";

    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

app.UseCors("AllowAll");

// Serve uploaded job posting attachments directly (binary content must bypass
// ResponseWrappingMiddleware, which buffers and JSON-re-wraps every response body).
app.UseStaticFiles();

app.UseMiddleware<ResponseWrappingMiddleware>();

// Registered before Authentication/Impersonation/Authorization so an exception thrown inside
// any of those (e.g. a malformed claim) still gets the JSON error envelope instead of a raw
// unhandled 500 - previously this ran after them and never saw their exceptions at all.
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseMiddleware<ImpersonationMiddleware>();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.Run();

// Exposed so WebApplicationFactory<Program> can bootstrap this app in integration/smoke tests.
public partial class Program { }
