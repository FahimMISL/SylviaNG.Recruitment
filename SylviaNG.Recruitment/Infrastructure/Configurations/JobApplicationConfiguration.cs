using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.ToTable("JobApplications");
            builder.HasKey(a => a.JobApplicationId);

            builder.Property(a => a.CandidateName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.CandidateEmail)
                .HasMaxLength(200);

            builder.Property(a => a.CandidatePhone)
                .HasMaxLength(50);

            builder.Property(a => a.CandidateNationalId)
                .HasMaxLength(50);

            builder.Property(a => a.ResumeUrl)
                .HasMaxLength(500);

            builder.Property(a => a.CoverLetter)
                .HasColumnType("text");

            builder.Property(a => a.ResumeExtractedText)
                .HasColumnType("text");

            builder.Property(a => a.ApplicationStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.Source)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(ApplicationSourceEnum.External);

            // Indexes
            builder.HasIndex(a => a.JobPostingId);
            builder.HasIndex(a => new { a.CandidateEmail, a.JobPostingId }).IsUnique();
            builder.HasIndex(a => a.CandidateProfileId);
            builder.HasIndex(a => a.WaiverRuleId);
            builder.HasIndex(a => a.SpecialCategoryId);
            builder.HasIndex(a => a.ReferralSourceId);

            // Relationships
            builder.HasMany(a => a.Interviews)
                .WithOne(i => i.JobApplication)
                .HasForeignKey(i => i.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // EP-17/US-127: a superseded/deleted rule shouldn't block cleanup - WaivedAt + the
            // ApplicationStatusHistory audit note preserve the "why" independent of the live FK.
            builder.HasOne(a => a.WaiverRule)
                .WithMany()
                .HasForeignKey(a => a.WaiverRuleId)
                .OnDelete(DeleteBehavior.SetNull);

            // SpecialCategory/ReferralSource feed rule-matching at submit time, not just display -
            // block accidental lookup-value deletion instead of silently orphaning applications.
            builder.HasOne(a => a.SpecialCategory)
                .WithMany()
                .HasForeignKey(a => a.SpecialCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.ReferralSource)
                .WithMany()
                .HasForeignKey(a => a.ReferralSourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
