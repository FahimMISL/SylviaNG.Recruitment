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

            builder.Property(s => s.StageType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.HasIndex(s => new { s.AssessmentWorkflowId, s.DisplayOrder });
        }
    }
}
