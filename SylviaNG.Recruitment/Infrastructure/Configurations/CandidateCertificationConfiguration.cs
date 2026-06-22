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

            builder.Property(c => c.CertificationName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.IssuingOrganization)
                .HasMaxLength(200);

            builder.Property(c => c.CredentialId)
                .HasMaxLength(200);

            builder.Property(c => c.CredentialUrl)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(c => c.CandidateId);
        }
    }
}
