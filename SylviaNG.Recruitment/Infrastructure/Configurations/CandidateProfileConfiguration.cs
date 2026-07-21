using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateProfileConfiguration : IEntityTypeConfiguration<CandidateProfile>
    {
        public void Configure(EntityTypeBuilder<CandidateProfile> builder)
        {
            builder.ToTable("CandidateProfiles");
            builder.HasKey(c => c.CandidateProfileId);

            builder.Property(c => c.KeycloakSubjectId).IsRequired().HasMaxLength(100);
            builder.Property(c => c.FullName).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Email).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Gender).HasMaxLength(20);
            builder.Property(c => c.NationalId).HasMaxLength(50);
            builder.Property(c => c.FatherName).HasMaxLength(200);
            builder.Property(c => c.MotherName).HasMaxLength(200);
            builder.Property(c => c.MaritalStatus).HasMaxLength(20);
            builder.Property(c => c.Religion).HasMaxLength(50);
            builder.Property(c => c.Nationality).HasMaxLength(100);
            builder.Property(c => c.Phone).HasMaxLength(50);
            builder.Property(c => c.PresentAddress).HasMaxLength(500);
            builder.Property(c => c.PermanentAddress).HasMaxLength(500);
            builder.Property(c => c.ProfilePhotoPath).HasMaxLength(500);
            builder.Property(c => c.SignaturePath).HasMaxLength(500);
            builder.Property(c => c.PrepopulatedFullName).HasMaxLength(200);
            builder.Property(c => c.PrepopulatedPhone).HasMaxLength(50);

            builder.HasIndex(c => c.KeycloakSubjectId).IsUnique();
        }
    }
}
