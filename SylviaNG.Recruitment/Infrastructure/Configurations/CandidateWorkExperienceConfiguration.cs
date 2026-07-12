using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateWorkExperienceConfiguration : IEntityTypeConfiguration<CandidateWorkExperience>
    {
        public void Configure(EntityTypeBuilder<CandidateWorkExperience> builder)
        {
            builder.ToTable("CandidateWorkExperiences");
            builder.HasKey(e => e.CandidateWorkExperienceId);

            builder.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Designation).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Responsibilities).IsRequired().HasColumnType("text");
            builder.Property(e => e.Location).HasMaxLength(200);

            builder.HasIndex(e => e.CandidateProfileId);

            builder.HasOne(e => e.CandidateProfile)
                .WithMany(c => c.WorkExperiences)
                .HasForeignKey(e => e.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
