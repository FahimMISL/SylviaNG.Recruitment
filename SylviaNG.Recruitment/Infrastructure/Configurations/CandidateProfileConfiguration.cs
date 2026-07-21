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
            builder.Property(c => c.Gender).HasConversion<string>().HasMaxLength(20);
            builder.Property(c => c.NationalId).HasMaxLength(50);
            builder.Property(c => c.FatherName).HasMaxLength(200);
            builder.Property(c => c.MotherName).HasMaxLength(200);
            builder.Property(c => c.MaritalStatus).HasConversion<string>().HasMaxLength(20);
            builder.Property(c => c.Religion).HasConversion<string>().HasMaxLength(50);
            builder.Property(c => c.Nationality).HasMaxLength(100);
            builder.Property(c => c.BloodGroup).HasConversion<string>().HasMaxLength(20);
            builder.Property(c => c.Phone).HasMaxLength(50);
            builder.Property(c => c.MobileOperator).HasConversion<string>().HasMaxLength(20);
            builder.Property(c => c.PresentAddressDetail).HasMaxLength(500);
            builder.Property(c => c.PermanentAddressDetail).HasMaxLength(500);
            builder.Property(c => c.ProfilePhotoPath).HasMaxLength(500);
            builder.Property(c => c.SignaturePath).HasMaxLength(500);
            builder.Property(c => c.PrepopulatedFullName).HasMaxLength(200);
            builder.Property(c => c.PrepopulatedPhone).HasMaxLength(50);

            builder.HasIndex(c => c.KeycloakSubjectId).IsUnique();

            // Present/Home address hierarchy - reference data only, no navigation back from
            // Division/District/Thana (a candidate profile isn't part of their aggregate).
            builder.HasOne<Division>().WithMany().HasForeignKey(c => c.PresentDivisionId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne<District>().WithMany().HasForeignKey(c => c.PresentDistrictId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne<Thana>().WithMany().HasForeignKey(c => c.PresentThanaId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne<Division>().WithMany().HasForeignKey(c => c.HomeDivisionId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne<District>().WithMany().HasForeignKey(c => c.HomeDistrictId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne<Thana>().WithMany().HasForeignKey(c => c.HomeThanaId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
