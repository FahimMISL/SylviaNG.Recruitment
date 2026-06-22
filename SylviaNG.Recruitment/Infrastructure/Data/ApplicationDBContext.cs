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

        // ── Existing ───────────────────────────────────────────
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<Employee> Employees { get; set; }

        // ── EPIC-01: Identity & Access ─────────────────────────
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<CandidateComplaint> CandidateComplaints { get; set; }
        public DbSet<ImpersonationLog> ImpersonationLogs { get; set; }

        // ── EPIC-02: Career Portal & Job Posting ───────────────
        public DbSet<JobPostingChannel> JobPostingChannels { get; set; }
        public DbSet<CareerContent> CareerContents { get; set; }

        // ── EPIC-03: Candidate Registration ────────────────────
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<CandidateEducation> CandidateEducations { get; set; }
        public DbSet<CandidateExperience> CandidateExperiences { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }
        public DbSet<CandidateCertification> CandidateCertifications { get; set; }
        public DbSet<CandidateDocument> CandidateDocuments { get; set; }
        public DbSet<CvParsingResult> CvParsingResults { get; set; }

        // ── Phase 2: Hiring Pipeline ───────────────────────────
        public DbSet<HiringPipelineStage> HiringPipelineStages { get; set; }
        public DbSet<CandidateStageProgress> CandidateStageProgresses { get; set; }

        // ── EPIC-04: Requisition Management ────────────────────
        public DbSet<Requisition> Requisitions { get; set; }
        public DbSet<RequisitionAttachment> RequisitionAttachments { get; set; }
        public DbSet<RequisitionApproval> RequisitionApprovals { get; set; }
        public DbSet<RequisitionStageConfig> RequisitionStageConfigs { get; set; }
        public DbSet<RequisitionTemplate> RequisitionTemplates { get; set; }

        // ── EPIC-05: Application & Candidate Tracking ──────────
        public DbSet<ApplicationScreeningResult> ApplicationScreeningResults { get; set; }
        public DbSet<TalentPool> TalentPools { get; set; }
        public DbSet<TalentPoolCandidate> TalentPoolCandidates { get; set; }
        public DbSet<ApplicationDuplicate> ApplicationDuplicates { get; set; }

        // ── EPIC-06: Referral Management ───────────────────────
        public DbSet<RecruitmentAgency> RecruitmentAgencies { get; set; }
        public DbSet<Referral> Referrals { get; set; }
        public DbSet<ReferralDuplicate> ReferralDuplicates { get; set; }

        // ── EPIC-07: Shortlisting ──────────────────────────────
        public DbSet<ShortlistFilter> ShortlistFilters { get; set; }
        public DbSet<ShortlistFilterCriteria> ShortlistFilterCriteria { get; set; }
        public DbSet<SavedSearch> SavedSearches { get; set; }

        // ── EPIC-08: Assessment & Evaluation ───────────────────
        public DbSet<AssessmentWorkflow> AssessmentWorkflows { get; set; }
        public DbSet<AssessmentStage> AssessmentStages { get; set; }
        public DbSet<QuestionGroup> QuestionGroups { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamCandidate> ExamCandidates { get; set; }
        public DbSet<ExamAnswer> ExamAnswers { get; set; }
        public DbSet<ExamHall> ExamHalls { get; set; }
        public DbSet<ExamSeatPlan> ExamSeatPlans { get; set; }
        public DbSet<AdmitCard> AdmitCards { get; set; }

        // ── EPIC-09: Interview Management ──────────────────────
        public DbSet<InterviewSession> InterviewSessions { get; set; }
        public DbSet<InterviewScorecard> InterviewScorecards { get; set; }
        public DbSet<InterviewScorecardCriteria> InterviewScorecardCriteria { get; set; }
        public DbSet<InterviewEvaluation> InterviewEvaluations { get; set; }
        public DbSet<InterviewEvaluationScore> InterviewEvaluationScores { get; set; }
        public DbSet<InterviewVenue> InterviewVenues { get; set; }

        // ── EPIC-10: Notifications ─────────────────────────────
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<NotificationEvent> NotificationEvents { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }

        // ── EPIC-11: Document & Letter Generation ──────────────
        public DbSet<DocumentTemplate> DocumentTemplates { get; set; }
        public DbSet<DocumentTemplateVersion> DocumentTemplateVersions { get; set; }
        public DbSet<GeneratedDocument> GeneratedDocuments { get; set; }
        public DbSet<DocumentAcceptance> DocumentAcceptances { get; set; }

        // ── EPIC-12: Pre-Employment Verification ───────────────
        public DbSet<VerificationWorkflow> VerificationWorkflows { get; set; }
        public DbSet<VerificationItem> VerificationItems { get; set; }
        public DbSet<MedicalTest> MedicalTests { get; set; }

        // ── EPIC-13: Pre-Boarding ──────────────────────────────
        public DbSet<PreBoardingProfile> PreBoardingProfiles { get; set; }
        public DbSet<Nominee> Nominees { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<InsuranceDetail> InsuranceDetails { get; set; }

        // ── EPIC-14: Fitment & Compensation ────────────────────
        public DbSet<FitmentData> FitmentData { get; set; }
        public DbSet<OfferCompensation> OfferCompensations { get; set; }

        // ── EPIC-15: Onboarding Integration ────────────────────
        public DbSet<OnboardingRecord> OnboardingRecords { get; set; }
        public DbSet<FinalSelectionPool> FinalSelectionPools { get; set; }
        public DbSet<JoiningBooklet> JoiningBooklets { get; set; }

        // ── EPIC-16: Export ────────────────────────────────────
        public DbSet<ExportRequest> ExportRequests { get; set; }

        // ── EPIC-17: Analytics & Dashboard ─────────────────────
        public DbSet<SavedReport> SavedReports { get; set; }
        public DbSet<DashboardWidget> DashboardWidgets { get; set; }

        // ── EPIC-18: System Integrations ───────────────────────
        public DbSet<IntegrationConfig> IntegrationConfigs { get; set; }
        public DbSet<IntegrationLog> IntegrationLogs { get; set; }

        // ── User Profile & Notifications ──────────────────────
        public DbSet<UserProfilePhoto> UserProfilePhotos { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }

        // ── Phase 1+2 Additions ──────────────────────────────
        public DbSet<ProfileFieldConfig> ProfileFieldConfigs { get; set; }
        public DbSet<EmailOtp> EmailOtps { get; set; }

        // ── Phase 3: Payment ─────────────────────────────────
        public DbSet<ApplicationFee> ApplicationFees { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        #endregion


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<SharedKernel.Audit.Audit>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt ??= DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

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
