using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class FinalSelectionPoolConfiguration : IEntityTypeConfiguration<FinalSelectionPool>
    {
        public void Configure(EntityTypeBuilder<FinalSelectionPool> builder)
        {
            builder.ToTable("FinalSelectionPools");
            builder.HasKey(f => f.FinalSelectionPoolId);

            builder.Property(f => f.JoiningBatch)
                .HasMaxLength(100);

            builder.Property(f => f.OnboardingChecklistJson)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(f => f.CandidateId);
            builder.HasIndex(f => f.JobApplicationId);

            // Relationships
            builder.HasOne(f => f.Candidate)
                .WithMany()
                .HasForeignKey(f => f.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.JobApplication)
                .WithMany()
                .HasForeignKey(f => f.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
