using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class PipelineStageConfiguration : IEntityTypeConfiguration<PipelineStage>
    {
        public void Configure(EntityTypeBuilder<PipelineStage> builder)
        {
            builder.ToTable("PipelineStages");
            builder.HasKey(s => s.PipelineStageId);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.StageType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Description)
                .HasColumnType("text");

            builder.Property(s => s.PassingCriteria)
                .HasMaxLength(500);

            builder.Property(s => s.ColorBadge)
                .HasMaxLength(20);

            builder.Property(s => s.EmailTemplate)
                .HasColumnType("text");

            builder.Property(s => s.RequiredDocuments)
                .HasMaxLength(1000);

            builder.Property(s => s.AutoProgressionRule)
                .HasMaxLength(500);

            builder.HasIndex(s => new { s.HiringPipelineId, s.DisplayOrder });

            builder.HasMany(s => s.Interviewers)
                .WithOne(i => i.PipelineStage)
                .HasForeignKey(i => i.PipelineStageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
