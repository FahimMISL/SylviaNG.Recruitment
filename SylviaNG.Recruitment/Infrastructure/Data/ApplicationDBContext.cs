using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Events;

namespace SylviaNG.Recruitment.Infrastructure.Data
{
    public class ApplicationDBContext : DbContext
    {
        private readonly IMultiTenantContextAccessor<MultiTenancy.TenantInfo>? _multiTenantContextAccessor;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDBContext(
            DbContextOptions<ApplicationDBContext> options,
            IMultiTenantContextAccessor<MultiTenancy.TenantInfo>? multiTenantContextAccessor = null,
            IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _multiTenantContextAccessor = multiTenantContextAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the current tenant ID with priority order:
        /// 1. JWT claims from HttpContext (HTTP requests)
        /// 2. Finbuckle TenantInfo
        /// 3. Empty string fallback
        /// This property is evaluated at query execution time for runtime filtering.
        /// </summary>
        public string CurrentTenantId
        {
            get
            {
                // Priority 1: JWT claims from HTTP request
                if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true)
                {
                    var tenantClaim = _httpContextAccessor.HttpContext.User.FindFirst("tenant_id");
                    if (tenantClaim != null && !string.IsNullOrEmpty(tenantClaim.Value))
                    {
                        return tenantClaim.Value;
                    }
                }

                // Priority 2: Finbuckle TenantInfo
                if (_multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Identifier != null)
                {
                    return _multiTenantContextAccessor.MultiTenantContext.TenantInfo.Identifier;
                }

                // Priority 3: Fallback
                return string.Empty;
            }
        }

        #region Tables

        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<JobPostingAttachment> JobPostingAttachments { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<HiringPipeline> HiringPipelines { get; set; }
        public DbSet<PipelineStage> PipelineStages { get; set; }
        public DbSet<PipelineStageInterviewer> PipelineStageInterviewers { get; set; }
        public DbSet<CandidateProfile> CandidateProfiles { get; set; }
        public DbSet<CandidateEducation> CandidateEducations { get; set; }
        public DbSet<CandidateWorkExperience> CandidateWorkExperiences { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }
        public DbSet<SkillLibraryItem> SkillLibraryItems { get; set; }
        public DbSet<UniversityLibraryItem> UniversityLibraryItems { get; set; }
        public DbSet<CandidateCertification> CandidateCertifications { get; set; }
        public DbSet<CandidateDocument> CandidateDocuments { get; set; }
        public DbSet<StaffProfile> StaffProfiles { get; set; }
        public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }
        public DbSet<ApplicationStatusReason> ApplicationStatusReasons { get; set; }
        public DbSet<JobApplicationStageProgress> JobApplicationStageProgresses { get; set; }
        public DbSet<ShortlistFilter> ShortlistFilters { get; set; }
        public DbSet<ShortlistFilterCriterion> ShortlistFilterCriteria { get; set; }
        public DbSet<AutoShortlistRun> AutoShortlistRuns { get; set; }
        public DbSet<AutoShortlistResult> AutoShortlistResults { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        public DbSet<CandidateRecommendation> CandidateRecommendations { get; set; }
        public DbSet<CandidateTalentPool> CandidateTalentPools { get; set; }
        public DbSet<QuestionGroup> QuestionGroups { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<ExamQuestionOption> ExamQuestionOptions { get; set; }
        public DbSet<SavedSearch> SavedSearches { get; set; }
        public DbSet<TalentPool> TalentPools { get; set; }
        public DbSet<TalentPoolCandidate> TalentPoolCandidates { get; set; }
        public DbSet<ExamVenue> ExamVenues { get; set; }
        public DbSet<ExamRoom> ExamRooms { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamEnrollment> ExamEnrollments { get; set; }
        public DbSet<ExamAnswer> ExamAnswers { get; set; }
        public DbSet<InterviewVenue> InterviewVenues { get; set; }
        public DbSet<InterviewRoom> InterviewRooms { get; set; }
        public DbSet<InterviewPanelMember> InterviewPanelMembers { get; set; }
        public DbSet<Scorecard> Scorecards { get; set; }
        public DbSet<ScorecardCriterion> ScorecardCriteria { get; set; }
        public DbSet<InterviewEvaluation> InterviewEvaluations { get; set; }
        public DbSet<InterviewEvaluationScore> InterviewEvaluationScores { get; set; }
        public DbSet<InterviewRoundConfig> InterviewRoundConfigs { get; set; }
        public DbSet<InterviewRoundConfigPanelMember> InterviewRoundConfigPanelMembers { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Thana> Thanas { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<EducationBoard> EducationBoards { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<MaritalStatus> MaritalStatuses { get; set; }
        public DbSet<Religion> Religions { get; set; }
        public DbSet<BloodGroup> BloodGroups { get; set; }
        public DbSet<MajorSubjectSscHsc> MajorSubjectsSscHsc { get; set; }
        public DbSet<MajorSubjectUniversity> MajorSubjectsUniversity { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<NotificationTemplateVersion> NotificationTemplateVersions { get; set; }
        public DbSet<EventTemplateMapping> EventTemplateMappings { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }
        public DbSet<DocumentTemplate> DocumentTemplates { get; set; }
        public DbSet<DocumentTemplateVersion> DocumentTemplateVersions { get; set; }
        public DbSet<CompanyBranding> CompanyBrandings { get; set; }
        public DbSet<OfferLetter> OfferLetters { get; set; }
        public DbSet<AppointmentLetter> AppointmentLetters { get; set; }
        public DbSet<JoiningBooklet> JoiningBooklets { get; set; }
        public DbSet<MedicalLetter> MedicalLetters { get; set; }
        public DbSet<TargetLetter> TargetLetters { get; set; }
        public DbSet<CandidateLoginOtp> CandidateLoginOtps { get; set; }
        public DbSet<FinalSelectionPool> FinalSelectionPools { get; set; }
        public DbSet<PreBoardingSubmission> PreBoardingSubmissions { get; set; }
        public DbSet<PreBoardingNominee> PreBoardingNominees { get; set; }
        public DbSet<FitmentData> FitmentDatas { get; set; }
        public DbSet<OfficeNote> OfficeNotes { get; set; }
        public DbSet<ExportRequest> ExportRequests { get; set; }
        public DbSet<WaiverRule> WaiverRules { get; set; }
        public DbSet<SpecialCategory> SpecialCategories { get; set; }
        public DbSet<ReferralSource> ReferralSources { get; set; }
        public DbSet<DashboardWidgetConfig> DashboardWidgetConfigs { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRoleAssignment> UserRoleAssignments { get; set; }
        public DbSet<ImpersonationSession> ImpersonationSessions { get; set; }
        public DbSet<ImpersonationLog> ImpersonationLogs { get; set; }
        public DbSet<ProfileFieldConfig> ProfileFieldConfigs { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDBContext).Assembly);

            // Ignore DomainEvents collection (used for in-memory event handling, not database persistence)
            modelBuilder.Ignore<DomainEvent>();
        }
    }
}
