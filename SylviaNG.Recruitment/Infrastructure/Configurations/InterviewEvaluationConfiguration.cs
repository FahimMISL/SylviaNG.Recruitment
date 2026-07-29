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

            builder.Property(e => e.OverallComments)
                .HasColumnType("text");

            builder.Property(e => e.SubmittedByUserName)
                .HasMaxLength(150);

            builder.HasOne(e => e.Interview)
                .WithMany()
                .HasForeignKey(e => e.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Scorecard)
                .WithMany()
                .HasForeignKey(e => e.ScorecardId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.InterviewId, e.EmployeeId }).IsUnique();

            builder.HasMany(e => e.Scores)
                .WithOne(s => s.InterviewEvaluation)
                .HasForeignKey(s => s.InterviewEvaluationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
