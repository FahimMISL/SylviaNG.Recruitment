using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class VerificationWorkflowConfiguration : IEntityTypeConfiguration<VerificationWorkflow>
    {
        public void Configure(EntityTypeBuilder<VerificationWorkflow> builder)
        {
            builder.ToTable("VerificationWorkflows");
            builder.HasKey(v => v.VerificationWorkflowId);

            builder.Property(v => v.OverallStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(v => v.CandidateId);
            builder.HasIndex(v => v.JobApplicationId);

            // Relationships
            builder.HasOne(v => v.Candidate)
                .WithMany()
                .HasForeignKey(v => v.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.JobApplication)
                .WithMany()
                .HasForeignKey(v => v.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.InitiatedByUser)
                .WithMany()
                .HasForeignKey(v => v.InitiatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.Items)
                .WithOne(i => i.VerificationWorkflow)
                .HasForeignKey(i => i.VerificationWorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.MedicalTest)
                .WithOne(m => m.VerificationWorkflow)
                .HasForeignKey<MedicalTest>(m => m.VerificationWorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
