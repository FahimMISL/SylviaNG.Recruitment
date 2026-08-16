using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateTalentPoolConfiguration : IEntityTypeConfiguration<CandidateTalentPool>
    {
        public void Configure(EntityTypeBuilder<CandidateTalentPool> builder)
        {
            builder.ToTable("CandidateTalentPools");
            builder.HasKey(t => t.CandidateTalentPoolId);

            // Multi-tenant: uniqueness is per-company, not global - two different companies can
            // independently bucket the same candidate (they apply across companies), so a bare
            // CandidateProfileId-only unique index would incorrectly block the second company.
            builder.HasIndex(t => new { t.CompanyId, t.CandidateProfileId }).IsUnique();

            builder.HasOne(t => t.CandidateProfile)
                .WithMany()
                .HasForeignKey(t => t.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
