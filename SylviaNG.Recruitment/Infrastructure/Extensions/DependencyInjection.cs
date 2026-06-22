using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Finbuckle.MultiTenant.Extensions;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.Infrastructure.Interceptors;
using SylviaNG.Recruitment.Infrastructure.Kafka;
using SylviaNG.Recruitment.Infrastructure.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
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

                options.ConfigureWarnings(w =>
                    w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

                // Apply audit interceptor once (works with any database)
                options.AddInterceptors(sp.GetRequiredService<UtcDateTimeInterceptor>());
            });

            // Register your repositories here
            // Adding DI of repositories
            // EPIC-01: Identity & Access
            services.AddScoped<IJobPostingRepository, JobPostingRepository>();
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<ICandidateComplaintRepository, CandidateComplaintRepository>();
            services.AddScoped<IImpersonationLogRepository, ImpersonationLogRepository>();

            // EPIC-02: Career Portal
            services.AddScoped<IJobPostingChannelRepository, JobPostingChannelRepository>();
            services.AddScoped<ICareerContentRepository, CareerContentRepository>();

            // EPIC-03: Candidate Registration
            services.AddScoped<ICandidateRepository, CandidateRepository>();
            services.AddScoped<ICandidateEducationRepository, CandidateEducationRepository>();
            services.AddScoped<ICandidateExperienceRepository, CandidateExperienceRepository>();
            services.AddScoped<ICandidateCertificationRepository, CandidateCertificationRepository>();
            services.AddScoped<ICandidateSkillRepository, CandidateSkillRepository>();
            services.AddScoped<ICandidateDocumentRepository, CandidateDocumentRepository>();

            // Phase 2: Hiring Pipeline
            services.AddScoped<IHiringPipelineStageRepository, HiringPipelineStageRepository>();

            // EPIC-04: Requisition Management
            services.AddScoped<IRequisitionRepository, RequisitionRepository>();
            services.AddScoped<IRequisitionApprovalRepository, RequisitionApprovalRepository>();
            services.AddScoped<IRequisitionAttachmentRepository, RequisitionAttachmentRepository>();
            services.AddScoped<IRequisitionStageConfigRepository, RequisitionStageConfigRepository>();
            services.AddScoped<IRequisitionTemplateRepository, RequisitionTemplateRepository>();

            // EPIC-05: Application Tracking
            services.AddScoped<IApplicationScreeningResultRepository, ApplicationScreeningResultRepository>();
            services.AddScoped<ITalentPoolRepository, TalentPoolRepository>();
            services.AddScoped<ITalentPoolCandidateRepository, TalentPoolCandidateRepository>();
            services.AddScoped<IApplicationDuplicateRepository, ApplicationDuplicateRepository>();

            // EPIC-06: Referral
            services.AddScoped<IRecruitmentAgencyRepository, RecruitmentAgencyRepository>();
            services.AddScoped<IReferralRepository, ReferralRepository>();
            services.AddScoped<IReferralDuplicateRepository, ReferralDuplicateRepository>();

            // EPIC-07: Shortlisting
            services.AddScoped<IShortlistFilterRepository, ShortlistFilterRepository>();
            services.AddScoped<IShortlistFilterCriteriaRepository, ShortlistFilterCriteriaRepository>();
            services.AddScoped<ISavedSearchRepository, SavedSearchRepository>();

            // EPIC-08: Assessment
            services.AddScoped<IAssessmentWorkflowRepository, AssessmentWorkflowRepository>();
            services.AddScoped<IAssessmentStageRepository, AssessmentStageRepository>();
            services.AddScoped<IQuestionGroupRepository, QuestionGroupRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IQuestionOptionRepository, QuestionOptionRepository>();
            services.AddScoped<IExamRepository, ExamRepository>();
            services.AddScoped<IExamCandidateRepository, ExamCandidateRepository>();
            services.AddScoped<IExamAnswerRepository, ExamAnswerRepository>();
            services.AddScoped<IExamHallRepository, ExamHallRepository>();
            services.AddScoped<IExamSeatPlanRepository, ExamSeatPlanRepository>();
            services.AddScoped<IAdmitCardRepository, AdmitCardRepository>();

            // EPIC-09: Interview
            services.AddScoped<IInterviewSessionRepository, InterviewSessionRepository>();
            services.AddScoped<IInterviewScorecardRepository, InterviewScorecardRepository>();
            services.AddScoped<IInterviewScorecardCriteriaRepository, InterviewScorecardCriteriaRepository>();
            services.AddScoped<IInterviewEvaluationRepository, InterviewEvaluationRepository>();
            services.AddScoped<IInterviewEvaluationScoreRepository, InterviewEvaluationScoreRepository>();
            services.AddScoped<IInterviewVenueRepository, InterviewVenueRepository>();

            // EPIC-10: Notifications
            services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
            services.AddScoped<INotificationEventRepository, NotificationEventRepository>();
            services.AddScoped<INotificationLogRepository, NotificationLogRepository>();

            // EPIC-11: Documents
            services.AddScoped<IDocumentTemplateRepository, DocumentTemplateRepository>();
            services.AddScoped<IDocumentTemplateVersionRepository, DocumentTemplateVersionRepository>();
            services.AddScoped<IGeneratedDocumentRepository, GeneratedDocumentRepository>();
            services.AddScoped<IDocumentAcceptanceRepository, DocumentAcceptanceRepository>();

            // EPIC-12: Verification
            services.AddScoped<IVerificationWorkflowRepository, VerificationWorkflowRepository>();
            services.AddScoped<IVerificationItemRepository, VerificationItemRepository>();
            services.AddScoped<IMedicalTestRepository, MedicalTestRepository>();

            // EPIC-13: Pre-Boarding
            services.AddScoped<IPreBoardingProfileRepository, PreBoardingProfileRepository>();
            services.AddScoped<INomineeRepository, NomineeRepository>();
            services.AddScoped<IEmergencyContactRepository, EmergencyContactRepository>();
            services.AddScoped<IInsuranceDetailRepository, InsuranceDetailRepository>();

            // EPIC-14: Fitment
            services.AddScoped<IFitmentDataRepository, FitmentDataRepository>();
            services.AddScoped<IOfferCompensationRepository, OfferCompensationRepository>();

            // EPIC-15: Onboarding
            services.AddScoped<IOnboardingRecordRepository, OnboardingRecordRepository>();
            services.AddScoped<IFinalSelectionPoolRepository, FinalSelectionPoolRepository>();
            services.AddScoped<IJoiningBookletRepository, JoiningBookletRepository>();

            // EPIC-16: Export
            services.AddScoped<IExportRequestRepository, ExportRequestRepository>();

            // EPIC-17: Analytics
            services.AddScoped<ISavedReportRepository, SavedReportRepository>();
            services.AddScoped<IDashboardWidgetRepository, DashboardWidgetRepository>();

            // EPIC-18: Integrations
            services.AddScoped<IIntegrationConfigRepository, IntegrationConfigRepository>();
            services.AddScoped<IIntegrationLogRepository, IntegrationLogRepository>();

            // User Profile & Notifications
            services.AddScoped<IUserProfilePhotoRepository, UserProfilePhotoRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();

            // Profile Field Configuration
            services.AddScoped<IProfileFieldConfigRepository, ProfileFieldConfigRepository>();

            // SMS Service (mock — swap for real provider later)
            services.AddScoped<ISmsService, MockSmsService>();

            // SSLCommerz Payment
            services.Configure<SslCommerzSettings>(configuration.GetSection("SslCommerz"));
            services.AddHttpClient("SslCommerz");
            services.AddScoped<ISslCommerzService, SslCommerzService>();

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Keycloak Admin Service
            services.AddSingleton<IKeycloakAdminService, KeycloakAdminService>();

            // CV Parsing Service
            services.AddSingleton<Application.Interfaces.Services.ICvParsingService, Application.Services.CvParsingService>();

            // AI Candidate Ranking Service
            services.AddScoped<Application.Interfaces.Services.ICandidateRankingService, Application.Services.CandidateRankingService>();

            // Kafka
            services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
            services.AddHostedService<EmployeeEventConsumer>();
            services.AddSingleton<IRecruitmentEventProducer, RecruitmentEventProducer>();
            services.AddHostedService<NotificationEventConsumer>();

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
