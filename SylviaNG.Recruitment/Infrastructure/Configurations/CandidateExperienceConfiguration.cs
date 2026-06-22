using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateExperienceConfiguration : IEntityTypeConfiguration<CandidateExperience>
    {
        public void Configure(EntityTypeBuilder<CandidateExperience> builder)
        {
            builder.ToTable("CandidateExperiences");
            builder.HasKey(e => e.CandidateExperienceId);

            builder.Property(e => e.OrganizationName)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(e => e.Designation)
                .HasMaxLength(200);

            builder.Property(e => e.Department)
                .HasMaxLength(200);

            builder.Property(e => e.Responsibilities)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(e => e.CandidateId);
        }
    }
}
