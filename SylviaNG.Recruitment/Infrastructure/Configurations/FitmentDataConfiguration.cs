using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class FitmentDataConfiguration : IEntityTypeConfiguration<FitmentData>
    {
        public void Configure(EntityTypeBuilder<FitmentData> builder)
        {
            builder.ToTable("FitmentData");
            builder.HasKey(f => f.FitmentDataId);

            builder.Property(f => f.ProposedGrade)
                .HasMaxLength(50);

            builder.Property(f => f.ProposedRole)
                .HasMaxLength(200);

            builder.Property(f => f.Location)
                .HasMaxLength(200);

            builder.Property(f => f.SalaryStructureJson)
                .HasColumnType("text");

            builder.Property(f => f.PayrollSource)
                .HasMaxLength(100);

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
