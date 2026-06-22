using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ApplicationScreeningResultConfiguration : IEntityTypeConfiguration<ApplicationScreeningResult>
    {
        public void Configure(EntityTypeBuilder<ApplicationScreeningResult> builder)
        {
            builder.ToTable("ApplicationScreeningResults");
            builder.HasKey(s => s.ApplicationScreeningResultId);

            builder.Property(s => s.RelevanceScore)
                .HasColumnType("decimal(5,2)");

            builder.Property(s => s.MatchedKeywordsJson)
                .HasColumnType("text");

            builder.Property(s => s.SkillTagsJson)
                .HasColumnType("text");

            builder.Property(s => s.ExperienceBand)
                .HasMaxLength(50);

            builder.Property(s => s.ScoreExplanation)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(s => s.JobApplicationId).IsUnique();
        }
    }
}
