using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Finbuckle.MultiTenant.Extensions;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Externals;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.Infrastructure.Interceptors;
using SylviaNG.Recruitment.Infrastructure.Kafka;
using SylviaNG.Recruitment.Infrastructure.Repositories;
using SylviaNG.Recruitment.Infrastructure.Services;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add your infrastructure services here

            var databaseProvider = configuration["Database:Provider"];
            var connectionString = configuration["Database:ConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Database connection string is not configured.");

            // Initialize timezone from configuration
            var timezoneId = configuration["RegionalSettings:TimezoneId"]
                ?? throw new InvalidOperationException("RegionalSettings:TimezoneId is not configured.");
            DateTimeUtility.Initialize(timezoneId);

            // Configure Finbuckle Multi-Tenant with Claim strategy (extracts tenant_id from JWT)
            services.AddMultiTenant<MultiTenancy.TenantInfo>()
                .WithClaimStrategy("tenant_id")  // Extract tenant from JWT claim 'tenant_id'
                .WithInMemoryStore(options =>
                {
                    // Default tenant for fallback
                    options.IsCaseSensitive = false;
                });

            // Register Audit Infrastructure (database-agnostic)
            services.AddHttpContextAccessor();
            services.AddSingleton<UtcDateTimeInterceptor>();

            // Configure database provider with audit interceptor
            services.AddDbContext<ApplicationDBContext>((sp, options) =>
            {
                var provider = NormalizeDatabaseProvider(databaseProvider);

                switch (provider)
                {
                    case "postgresql":
                        options.UseNpgsql(connectionString);
                        break;
                    case "sqlserver":
                        options.UseSqlServer(connectionString);
                        break;
                    case "oracle":
                        options.UseOracle(connectionString);
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Unsupported database provider: {databaseProvider}. Supported providers: PostgreSQL, SqlServer, Oracle.");
                }

                // Apply audit interceptor once (works with any database)
                options.AddInterceptors(sp.GetRequiredService<UtcDateTimeInterceptor>());
            });

            // Register your repositories here
            // Adding DI of repositories
            services.AddScoped<IJobPostingRepository, JobPostingRepository>();
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            services.AddScoped<IJobPostingAttachmentRepository, JobPostingAttachmentRepository>();
            services.AddScoped<IHiringPipelineRepository, HiringPipelineRepository>();
            services.AddScoped<IJobApplicationStageProgressRepository, JobApplicationStageProgressRepository>();
            services.AddScoped<IShortlistFilterRepository, ShortlistFilterRepository>();
            services.AddScoped<ICandidateProfileRepository, CandidateProfileRepository>();
            services.AddScoped<ICandidateEducationRepository, CandidateEducationRepository>();
            services.AddScoped<ICandidateWorkExperienceRepository, CandidateWorkExperienceRepository>();
            services.AddScoped<ICandidateSkillRepository, CandidateSkillRepository>();
            services.AddScoped<ISkillLibraryItemRepository, SkillLibraryItemRepository>();
            services.AddScoped<ICandidateCertificationRepository, CandidateCertificationRepository>();
            services.AddScoped<ICandidateDocumentRepository, CandidateDocumentRepository>();
            services.AddScoped<IStaffProfileRepository, StaffProfileRepository>();
            services.AddScoped<IApplicationStatusReasonRepository, ApplicationStatusReasonRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IApplicationSettingRepository, ApplicationSettingRepository>();

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // File storage (local disk, backs job posting attachments)
            services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            // File storage (local disk, backs career-portal / internal-job-board CV uploads)
            services.Configure<ApplicationCvStorageSettings>(configuration.GetSection(ApplicationCvStorageSettings.SectionName));
            services.AddScoped<IApplicationCvStorageService, LocalApplicationCvStorageService>();

            // Size/extension policy for candidate profile photo/signature and document uploads
            // (both reuse the IFileStorageService/FileStorageSettings root above - see plan decision).
            services.Configure<CandidatePhotoSignatureSettings>(configuration.GetSection(CandidatePhotoSignatureSettings.SectionName));
            services.Configure<CandidateDocumentSettings>(configuration.GetSection(CandidateDocumentSettings.SectionName));

            // Free, local, regex/heuristic resume parsing (no external AI API) - see plan decision.
            services.AddScoped<IResumeParsingService, ResumeParsingService>();

            // Keycloak REST client (login token proxy + Admin REST user registration)
            services.Configure<KeycloakSettings>(configuration.GetSection(KeycloakSettings.SectionName));
            services.AddHttpClient<IKeycloakClient, KeycloakClient>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            // SSLCommerz payment gateway client (EP-17) - bounded timeout since InitiateAsync now
            // runs inline inside the apply-form submit request, alongside multipart CV parsing.
            services.Configure<SslCommerzSettings>(configuration.GetSection(SslCommerzSettings.SectionName));
            services.AddHttpClient<ISslCommerzPaymentGateway, SslCommerzPaymentGateway>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            // Kafka — disabled, no broker reachable at configured BootstrapServers
            // services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
            // services.AddHostedService<EmployeeEventConsumer>();

            return services;
        }

        private static string NormalizeDatabaseProvider(string? provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentNullException(nameof(provider), "Database provider is not specified.");

            return provider.Trim().ToLowerInvariant();
        }
    }
}
