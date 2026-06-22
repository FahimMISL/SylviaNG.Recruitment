using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class InterviewEvaluationScoreConfiguration : IEntityTypeConfiguration<InterviewEvaluationScore>
    {
        public void Configure(EntityTypeBuilder<InterviewEvaluationScore> builder)
        {
            builder.ToTable("InterviewEvaluationScores");
            builder.HasKey(s => s.InterviewEvaluationScoreId);

            builder.Property(s => s.Notes)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(s => s.InterviewEvaluationId);
            builder.HasIndex(s => s.InterviewScorecardCriteriaId);

            // Relationships
            builder.HasOne(s => s.ScorecardCriteria)
                .WithMany()
                .HasForeignKey(s => s.InterviewScorecardCriteriaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
