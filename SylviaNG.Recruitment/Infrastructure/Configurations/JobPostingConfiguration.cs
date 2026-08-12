using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

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

            builder.Property(j => j.SalaryCurrency)
                .HasMaxLength(10);

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
            builder.HasIndex(j => j.Status);
            builder.HasIndex(j => j.Title).IsUnique();
            builder.HasIndex(j => j.JobPostingCode).IsUnique();
            builder.HasIndex(j => j.HiringPipelineId);
            builder.HasIndex(j => j.DepartmentId);

            // Relationships
            builder.HasMany(j => j.Applications)
                .WithOne(a => a.JobPosting)
                .HasForeignKey(a => a.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Note: JobPostingAttachment relationship (FK + cascade delete) is configured
            // in JobPostingAttachmentConfiguration.
        }
    }
}
