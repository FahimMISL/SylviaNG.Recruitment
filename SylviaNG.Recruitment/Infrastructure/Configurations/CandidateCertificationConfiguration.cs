using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateCertificationConfiguration : IEntityTypeConfiguration<CandidateCertification>
    {
        public void Configure(EntityTypeBuilder<CandidateCertification> builder)
        {
            builder.ToTable("CandidateCertifications");
            builder.HasKey(c => c.CandidateCertificationId);

            builder.Property(c => c.CertificationName).IsRequired().HasMaxLength(200);
            builder.Property(c => c.IssuingOrganization).HasMaxLength(200);
            builder.Property(c => c.CredentialId).HasMaxLength(100);
            builder.Property(c => c.CertificateFilePath).HasMaxLength(500);

            builder.HasIndex(c => c.CandidateProfileId);

            builder.HasOne(c => c.CandidateProfile)
                .WithMany(c => c.Certifications)
                .HasForeignKey(c => c.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
