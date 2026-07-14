using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class JobPostingConfiguration : IEntityTypeConfiguration<JobPosting>
    {
        public void Configure(EntityTypeBuilder<JobPosting> builder)
        {
            builder.ToTable("JobPostings");
            builder.HasKey(j => j.JobPostingId);

            builder.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.Description)
                .HasColumnType("text");

            builder.Property(j => j.Requirements)
                .HasColumnType("text");

            builder.Property(j => j.NumberOfPositions)
                .HasDefaultValue(1);

            builder.Property(j => j.EmploymentType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(j => j.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(j => j.MinSalary)
                .HasColumnType("decimal(18,2)");

            builder.Property(j => j.MaxSalary)
                .HasColumnType("decimal(18,2)");

            // EP-02: Job Vacancy Configuration fields
            builder.Property(j => j.JobPostingCode)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(j => j.Location)
                .HasMaxLength(200);

            builder.Property(j => j.CircularType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(j => j.MinEducationLevel)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(j => j.RequiredDistrict)
                .HasMaxLength(100);

            builder.Property(j => j.ApplicationFeeAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(j => j.ApplicationFeeCurrency)
                .HasMaxLength(10);

            // Indexes
            builder.HasIndex(j => j.SiteId);
            builder.HasIndex(j => j.Status);
            builder.HasIndex(j => new { j.SiteId, j.Title }).IsUnique();
            builder.HasIndex(j => j.JobPostingCode).IsUnique();
            builder.HasIndex(j => j.HiringPipelineId);

            // Relationships
            builder.HasMany(j => j.Applications)
                .WithOne(a => a.JobPosting)
                .HasForeignKey(a => a.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Note: JobPostingAttachment relationship (FK + cascade delete) is configured
            // in JobPostingAttachmentConfiguration.

            // Seed data
            builder.HasData(
                new
                {
                    JobPostingId = 1L,
                    SiteId = 1L,
                    DepartmentId = 1L,
                    DesignationId = 1L,
                    Title = "Senior Software Engineer",
                    Description = "We are looking for a Senior Software Engineer to join our team.",
                    Requirements = "5+ years of experience in .NET, C#, and SQL Server.",
                    NumberOfPositions = 2,
                    EmploymentType = EmploymentTypeEnum.FullTime,
                    Status = JobStatusEnum.Open,
                    MinSalary = 80000m,
                    MaxSalary = 120000m,
                    PostingDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ClosingDate = new DateTime(2025, 6, 30, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    JobPostingCode = "JOB-2026-000001",
                    CircularType = CircularTypeEnum.Both,
                    TenantId = "default_tenant",
                    Remarks = (string?)null,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = 1L,
                    UpdatedAt = (DateTime?)null,
                    UpdatedBy = (long?)null,
                    DeletedAt = (DateTime?)null,
                    DeletedBy = (long?)null,
                    AuditStatus = 1
                },
                new
                {
                    JobPostingId = 2L,
                    SiteId = 1L,
                    DepartmentId = 2L,
                    DesignationId = 2L,
                    Title = "UI/UX Designer",
                    Description = "Looking for a creative UI/UX Designer.",
                    Requirements = "3+ years of experience in Figma and Adobe XD.",
                    NumberOfPositions = 1,
                    EmploymentType = EmploymentTypeEnum.FullTime,
                    Status = JobStatusEnum.Open,
                    MinSalary = 50000m,
                    MaxSalary = 80000m,
                    PostingDate = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                    ClosingDate = new DateTime(2025, 7, 31, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    JobPostingCode = "JOB-2026-000002",
                    CircularType = CircularTypeEnum.Both,
                    TenantId = "default_tenant",
                    Remarks = (string?)null,
                    CreatedAt = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = 1L,
                    UpdatedAt = (DateTime?)null,
                    UpdatedBy = (long?)null,
                    DeletedAt = (DateTime?)null,
                    DeletedBy = (long?)null,
                    AuditStatus = 1
                }
            );
        }
    }
}
