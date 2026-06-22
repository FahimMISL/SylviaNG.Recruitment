using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using System.Reflection;

namespace SylviaNG.Recruitment.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Add your application services here

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            );

            services.AddFluentValidationAutoValidation()
               .AddValidatorsFromAssembly(typeof(Program).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Register your services here
            // Adding DI of services

            #region Services -> Business Classes
            // Add recruitment-specific services here

            // EPIC-01: Identity & Access
            services.AddScoped<IJobPostingService, JobPostingService>();
            services.AddScoped<IJobApplicationService, JobApplicationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICandidateComplaintService, CandidateComplaintService>();
            services.AddScoped<IImpersonationLogService, ImpersonationLogService>();

            // EPIC-02: Career Portal
            services.AddScoped<IJobPostingChannelService, JobPostingChannelService>();
            services.AddScoped<ICareerContentService, CareerContentService>();

            // EPIC-03: Candidate Registration
            services.AddScoped<ICandidateService, CandidateService>();
            services.AddScoped<ICandidateEducationService, CandidateEducationService>();
            services.AddScoped<ICandidateExperienceService, CandidateExperienceService>();
            services.AddScoped<ICandidateCertificationService, CandidateCertificationService>();
            services.AddScoped<ICandidateSkillService, CandidateSkillService>();
            services.AddScoped<ICandidateDocumentService, CandidateDocumentService>();

            // Phase 2: Hiring Pipeline
            services.AddScoped<IHiringPipelineStageService, HiringPipelineStageService>();

            // EPIC-04: Requisition Management
            services.AddScoped<IRequisitionService, RequisitionService>();
            services.AddScoped<IRequisitionApprovalService, RequisitionApprovalService>();
            services.AddScoped<IRequisitionAttachmentService, RequisitionAttachmentService>();
            services.AddScoped<IRequisitionStageConfigService, RequisitionStageConfigService>();
            services.AddScoped<IRequisitionTemplateService, RequisitionTemplateService>();

            // EPIC-05: Application Tracking
            services.AddScoped<IApplicationScreeningResultService, ApplicationScreeningResultService>();
            services.AddScoped<ITalentPoolService, TalentPoolService>();
            services.AddScoped<ITalentPoolCandidateService, TalentPoolCandidateService>();
            services.AddScoped<IApplicationDuplicateService, ApplicationDuplicateService>();

            // EPIC-06: Referral
            services.AddScoped<IRecruitmentAgencyService, RecruitmentAgencyService>();
            services.AddScoped<IReferralService, ReferralService>();
            services.AddScoped<IReferralDuplicateService, ReferralDuplicateService>();

            // EPIC-07: Shortlisting
            services.AddScoped<IShortlistFilterService, ShortlistFilterService>();
            services.AddScoped<IShortlistFilterCriteriaService, ShortlistFilterCriteriaService>();
            services.AddScoped<ISavedSearchService, SavedSearchService>();

            // EPIC-08: Assessment
            services.AddScoped<IAssessmentWorkflowService, AssessmentWorkflowService>();
            services.AddScoped<IAssessmentStageService, AssessmentStageService>();
            services.AddScoped<IQuestionGroupService, QuestionGroupService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IQuestionOptionService, QuestionOptionService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IExamCandidateService, ExamCandidateService>();
            services.AddScoped<IExamAnswerService, ExamAnswerService>();
            services.AddScoped<IExamHallService, ExamHallService>();
            services.AddScoped<IExamSeatPlanService, ExamSeatPlanService>();
            services.AddScoped<IAdmitCardService, AdmitCardService>();

            // EPIC-09: Interview
            services.AddScoped<IInterviewSessionService, InterviewSessionService>();
            services.AddScoped<IInterviewScorecardService, InterviewScorecardService>();
            services.AddScoped<IInterviewScorecardCriteriaService, InterviewScorecardCriteriaService>();
            services.AddScoped<IInterviewEvaluationService, InterviewEvaluationService>();
            services.AddScoped<IInterviewEvaluationScoreService, InterviewEvaluationScoreService>();
            services.AddScoped<IInterviewVenueService, InterviewVenueService>();

            // EPIC-10: Notifications
            services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
            services.AddScoped<INotificationEventService, NotificationEventService>();
            services.AddScoped<INotificationLogService, NotificationLogService>();

            // EPIC-11: Documents
            services.AddScoped<IDocumentTemplateService, DocumentTemplateService>();
            services.AddScoped<IDocumentTemplateVersionService, DocumentTemplateVersionService>();
            services.AddScoped<IGeneratedDocumentService, GeneratedDocumentService>();
            services.AddScoped<IDocumentAcceptanceService, DocumentAcceptanceService>();

            // EPIC-12: Verification
            services.AddScoped<IVerificationWorkflowService, VerificationWorkflowService>();
            services.AddScoped<IVerificationItemService, VerificationItemService>();
            services.AddScoped<IMedicalTestService, MedicalTestService>();

            // EPIC-13: Pre-Boarding
            services.AddScoped<IPreBoardingProfileService, PreBoardingProfileService>();
            services.AddScoped<INomineeService, NomineeService>();
            services.AddScoped<IEmergencyContactService, EmergencyContactService>();
            services.AddScoped<IInsuranceDetailService, InsuranceDetailService>();

            // EPIC-14: Fitment
            services.AddScoped<IFitmentDataService, FitmentDataService>();
            services.AddScoped<IOfferCompensationService, OfferCompensationService>();

            // EPIC-15: Onboarding
            services.AddScoped<IOnboardingRecordService, OnboardingRecordService>();
            services.AddScoped<IFinalSelectionPoolService, FinalSelectionPoolService>();
            services.AddScoped<IJoiningBookletService, JoiningBookletService>();

            // EPIC-16: Export
            services.AddScoped<IExportRequestService, ExportRequestService>();

            // EPIC-17: Analytics
            services.AddScoped<ISavedReportService, SavedReportService>();
            services.AddScoped<IDashboardWidgetService, DashboardWidgetService>();

            // EPIC-18: Integrations
            services.AddScoped<IIntegrationConfigService, IntegrationConfigService>();
            services.AddScoped<IIntegrationLogService, IntegrationLogService>();

            // User Profile & Notifications
            services.AddScoped<IUserProfilePhotoService, UserProfilePhotoService>();
            services.AddScoped<IUserNotificationService, UserNotificationService>();

            // Profile Field Configuration
            services.AddScoped<IProfileFieldConfigService, ProfileFieldConfigService>();

            // Eligibility Check
            services.AddScoped<IEligibilityCheckService, EligibilityCheckService>();

            // Email OTP
            services.AddScoped<IEmailOtpService, EmailOtpService>();

            // Provide access to HttpContext for request metadata enrichment
            services.AddHttpContextAccessor();
            #endregion

            return services;
        }
    }
}
