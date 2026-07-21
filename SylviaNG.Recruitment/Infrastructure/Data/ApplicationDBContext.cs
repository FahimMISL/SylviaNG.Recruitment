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
