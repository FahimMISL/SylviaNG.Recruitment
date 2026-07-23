using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class TalentPoolCandidateConfiguration : IEntityTypeConfiguration<TalentPoolCandidate>
    {
        public void Configure(EntityTypeBuilder<TalentPoolCandidate> builder)
        {
            builder.ToTable("TalentPoolCandidates");
            builder.HasKey(t => t.TalentPoolCandidateId);

            builder.HasIndex(t => new { t.TalentPoolId, t.CandidateProfileId }).IsUnique();

            builder.HasOne(t => t.TalentPool)
                .WithMany(p => p.Candidates)
                .HasForeignKey(t => t.TalentPoolId)
                .OnDelete(DeleteBehavior.Cascade);

            // No reverse nav on CandidateProfile - see TalentPoolCandidate's class summary.
            builder.HasOne(t => t.CandidateProfile)
                .WithMany()
                .HasForeignKey(t => t.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
