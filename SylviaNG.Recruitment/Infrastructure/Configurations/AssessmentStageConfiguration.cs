using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class AssessmentStageConfiguration : IEntityTypeConfiguration<AssessmentStage>
    {
        public void Configure(EntityTypeBuilder<AssessmentStage> builder)
        {
            builder.ToTable("AssessmentStages");
            builder.HasKey(s => s.AssessmentStageId);

            builder.Property(s => s.AssessmentType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(s => s.StageName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.PassThreshold)
                .HasColumnType("decimal(5,2)");

            builder.Property(s => s.Weight)
                .HasColumnType("decimal(5,2)");

            // Indexes
            builder.HasIndex(s => s.AssessmentWorkflowId);
            builder.HasIndex(s => new { s.AssessmentWorkflowId, s.StageOrder }).IsUnique();
        }
    }
}
