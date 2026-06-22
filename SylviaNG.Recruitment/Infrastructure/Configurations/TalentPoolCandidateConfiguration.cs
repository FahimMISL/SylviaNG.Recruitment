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
            builder.HasKey(tc => tc.TalentPoolCandidateId);

            // Indexes
            builder.HasIndex(tc => new { tc.TalentPoolId, tc.CandidateId }).IsUnique();

            // Relationships
            builder.HasOne(tc => tc.Candidate)
                .WithMany()
                .HasForeignKey(tc => tc.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
