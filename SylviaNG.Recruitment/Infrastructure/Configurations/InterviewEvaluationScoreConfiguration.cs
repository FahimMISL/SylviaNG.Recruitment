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

            builder.Property(s => s.Score)
                .HasColumnType("decimal(6,2)")
                .IsRequired();

            builder.HasOne(s => s.ScorecardCriterion)
                .WithMany()
                .HasForeignKey(s => s.ScorecardCriterionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.InterviewEvaluationId);
        }
    }
}
