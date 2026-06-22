using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class HiringPipelineStageConfiguration : IEntityTypeConfiguration<HiringPipelineStage>
    {
        public void Configure(EntityTypeBuilder<HiringPipelineStage> builder)
        {
            builder.ToTable("HiringPipelineStages");
            builder.HasKey(h => h.HiringPipelineStageId);

            builder.Property(h => h.StageName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.StageType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.Description)
                .HasMaxLength(1000);

            builder.HasOne(h => h.JobPosting)
                .WithMany(j => j.HiringPipelineStages)
                .HasForeignKey(h => h.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(h => h.JobPostingId);
        }
    }
}
