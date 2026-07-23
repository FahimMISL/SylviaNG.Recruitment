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

            services.AddScoped<IJobPostingService, JobPostingService>();
            services.AddScoped<IJobApplicationService, JobApplicationService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJobPostingAttachmentService, JobPostingAttachmentService>();
            services.AddScoped<IHiringPipelineService, HiringPipelineService>();
            services.AddScoped<IJobApplicationStageProgressService, JobApplicationStageProgressService>();
            services.AddScoped<IShortlistFilterService, ShortlistFilterService>();
            services.AddScoped<ICandidateRecommendationService, CandidateRecommendationService>();
            services.AddScoped<ISavedSearchService, SavedSearchService>();
            services.AddScoped<IShortlistFilterEvaluationService, ShortlistFilterEvaluationService>();
            services.AddScoped<IAutoShortlistRunService, AutoShortlistRunService>();
            services.AddScoped<ICurrentCandidateService, CurrentCandidateService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICandidateProfileService, CandidateProfileService>();
            services.AddScoped<ICandidateEducationService, CandidateEducationService>();
            services.AddScoped<ICandidateWorkExperienceService, CandidateWorkExperienceService>();
            services.AddScoped<ICandidateSkillService, CandidateSkillService>();
            services.AddScoped<ICandidateTagService, CandidateTagService>();
            services.AddScoped<ISkillLibraryService, SkillLibraryService>();
            services.AddScoped<IUniversityLibraryService, UniversityLibraryService>();
            services.AddScoped<ICandidateCertificationService, CandidateCertificationService>();
            services.AddScoped<ICandidateDocumentService, CandidateDocumentService>();
            services.AddScoped<IStaffProfileService, StaffProfileService>();
            services.AddScoped<IAccountSettingsService, AccountSettingsService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IApplicationSettingService, ApplicationSettingService>();
            services.AddScoped<IQuestionGroupService, QuestionGroupService>();
            services.AddScoped<IExamQuestionService, ExamQuestionService>();
            services.AddScoped<IExamQuestionImportService, ExamQuestionImportService>();
            services.AddScoped<ITalentPoolService, TalentPoolService>();
            services.AddScoped<IExamVenueService, ExamVenueService>();
            services.AddScoped<IExamRoomService, ExamRoomService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IExamEnrollmentService, ExamEnrollmentService>();
            services.AddScoped<IExamTakingService, ExamTakingService>();
            services.AddScoped<IExamScoreImportService, ExamScoreImportService>();
            services.AddScoped<IExamSeatPlanService, ExamSeatPlanService>();
            services.AddScoped<IExamNotificationService, ExamNotificationService>();
            services.AddScoped<IInterviewVenueService, InterviewVenueService>();
            services.AddScoped<IInterviewRoomService, InterviewRoomService>();
            services.AddScoped<IInterviewService, InterviewService>();
            services.AddScoped<IInterviewNotificationService, InterviewNotificationService>();
            services.AddScoped<IAddressLookupService, AddressLookupService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IEducationBoardService, EducationBoardService>();
            services.AddScoped<IDegreeService, DegreeService>();
            services.AddScoped<IGenderService, GenderService>();
            services.AddScoped<IMaritalStatusService, MaritalStatusService>();
            services.AddScoped<IReligionService, ReligionService>();
            services.AddScoped<IBloodGroupService, BloodGroupService>();
            services.AddScoped<IMajorSubjectSscHscService, MajorSubjectSscHscService>();
            services.AddScoped<IMajorSubjectUniversityService, MajorSubjectUniversityService>();

            // Provide access to HttpContext for request metadata enrichment
            services.AddHttpContextAccessor();
            #endregion

            return services;
        }
    }
}
