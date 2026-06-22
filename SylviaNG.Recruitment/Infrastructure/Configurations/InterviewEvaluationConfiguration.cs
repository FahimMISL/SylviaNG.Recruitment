using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class InterviewEvaluationConfiguration : IEntityTypeConfiguration<InterviewEvaluation>
    {
        public void Configure(EntityTypeBuilder<InterviewEvaluation> builder)
        {
            builder.ToTable("InterviewEvaluations");
            builder.HasKey(e => e.InterviewEvaluationId);

            builder.Property(e => e.OverallScore)
                .HasColumnType("decimal(5,2)");

            builder.Property(e => e.Recommendation)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.Commentary)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(e => e.InterviewId);
            builder.HasIndex(e => e.PanelistUserId);

            // Relationships
            builder.HasOne(e => e.PanelistUser)
                .WithMany()
                .HasForeignKey(e => e.PanelistUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Scores)
                .WithOne(s => s.InterviewEvaluation)
                .HasForeignKey(s => s.InterviewEvaluationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
