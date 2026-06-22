using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class MedicalTestConfiguration : IEntityTypeConfiguration<MedicalTest>
    {
        public void Configure(EntityTypeBuilder<MedicalTest> builder)
        {
            builder.ToTable("MedicalTests");
            builder.HasKey(m => m.MedicalTestId);

            builder.Property(m => m.FitnessStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(m => m.MedicalCenter)
                .HasMaxLength(200);

            builder.Property(m => m.ResultFileUrl)
                .HasMaxLength(500);

            builder.Property(m => m.Notes)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(m => m.VerificationWorkflowId).IsUnique();
        }
    }
}
