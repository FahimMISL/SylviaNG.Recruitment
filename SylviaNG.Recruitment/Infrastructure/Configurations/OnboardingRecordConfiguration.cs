using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class OnboardingRecordConfiguration : IEntityTypeConfiguration<OnboardingRecord>
    {
        public void Configure(EntityTypeBuilder<OnboardingRecord> builder)
        {
            builder.ToTable("OnboardingRecords");
            builder.HasKey(o => o.OnboardingRecordId);

            builder.Property(o => o.Stage)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(o => o.CoreHrEmployeeId)
                .HasMaxLength(100);

            builder.Property(o => o.PayrollReferenceId)
                .HasMaxLength(100);

            builder.Property(o => o.FailureDetails)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(o => o.CandidateId);
            builder.HasIndex(o => o.JobApplicationId);
            builder.HasIndex(o => o.Stage);

            // Relationships
            builder.HasOne(o => o.Candidate)
                .WithMany()
                .HasForeignKey(o => o.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.JobApplication)
                .WithMany()
                .HasForeignKey(o => o.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
