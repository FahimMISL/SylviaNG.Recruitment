using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class JobApplicationStageProgressConfiguration : IEntityTypeConfiguration<JobApplicationStageProgress>
    {
        public void Configure(EntityTypeBuilder<JobApplicationStageProgress> builder)
        {
            builder.ToTable("JobApplicationStageProgresses");
            builder.HasKey(p => p.JobApplicationStageProgressId);

            builder.Property(p => p.StageName).HasMaxLength(200);
            builder.Property(p => p.StageType).HasMaxLength(100);
            builder.Property(p => p.MeetingLink).HasMaxLength(500);
            builder.Property(p => p.LastUpdatedByUserName).HasMaxLength(200);

            builder.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(p => p.Notes)
                .HasColumnType("text");

            // PipelineStageId is intentionally NOT an FK - HiringPipelineService.UpdateAsync
            // clears and re-inserts PipelineStage rows on every pipeline edit, so a hard FK
            // here would either cascade-delete in-flight candidate progress or block edits
            // to any pipeline that already has a tracked candidate.
            builder.HasIndex(p => new { p.JobApplicationId, p.PipelineStageId }).IsUnique();

            builder.HasOne(p => p.JobApplication)
                .WithMany(a => a.StageProgress)
                .HasForeignKey(p => p.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
