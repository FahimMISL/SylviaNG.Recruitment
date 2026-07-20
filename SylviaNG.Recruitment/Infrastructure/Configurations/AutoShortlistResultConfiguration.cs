using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class AutoShortlistResultConfiguration : IEntityTypeConfiguration<AutoShortlistResult>
    {
        public void Configure(EntityTypeBuilder<AutoShortlistResult> builder)
        {
            builder.ToTable("AutoShortlistResults");
            builder.HasKey(r => r.AutoShortlistResultId);

            builder.Property(r => r.Explanation)
                .HasMaxLength(2000);

            builder.Property(r => r.MatchedSkills)
                .HasMaxLength(1000);

            builder.Property(r => r.ExperienceBand)
                .HasMaxLength(50);

            builder.Property(r => r.ScoringError)
                .HasMaxLength(500);

            builder.Property(r => r.HrOverrideDecision)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasIndex(r => new { r.AutoShortlistRunId, r.JobApplicationId })
                .IsUnique();
        }
    }
}
