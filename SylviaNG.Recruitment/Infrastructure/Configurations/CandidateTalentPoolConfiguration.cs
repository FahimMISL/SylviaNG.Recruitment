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

            builder.HasIndex(t => t.CandidateProfileId).IsUnique();

            builder.HasOne(t => t.CandidateProfile)
                .WithMany()
                .HasForeignKey(t => t.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
