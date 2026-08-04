using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class InterviewRoundConfigConfiguration : IEntityTypeConfiguration<InterviewRoundConfig>
    {
        public void Configure(EntityTypeBuilder<InterviewRoundConfig> builder)
        {
            builder.ToTable("InterviewRoundConfigs");
            builder.HasKey(r => r.InterviewRoundConfigId);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(r => new { r.JobPostingId, r.Sequence });

            builder.HasOne(r => r.JobPosting)
                .WithMany()
                .HasForeignKey(r => r.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Scorecard)
                .WithMany()
                .HasForeignKey(r => r.ScorecardId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.PanelMembers)
                .WithOne(p => p.InterviewRoundConfig)
                .HasForeignKey(p => p.InterviewRoundConfigId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
