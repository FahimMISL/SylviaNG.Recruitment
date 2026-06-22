using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class InterviewScorecardConfiguration : IEntityTypeConfiguration<InterviewScorecard>
    {
        public void Configure(EntityTypeBuilder<InterviewScorecard> builder)
        {
            builder.ToTable("InterviewScorecards");
            builder.HasKey(s => s.InterviewScorecardId);

            builder.Property(s => s.ScorecardName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Description)
                .HasMaxLength(500);

            builder.Property(s => s.ScoringScale)
                .HasMaxLength(100);

            // Relationships
            builder.HasMany(s => s.Criteria)
                .WithOne(c => c.InterviewScorecard)
                .HasForeignKey(c => c.InterviewScorecardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
