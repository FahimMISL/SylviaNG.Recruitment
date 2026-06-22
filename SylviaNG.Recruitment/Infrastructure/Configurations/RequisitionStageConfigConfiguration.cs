using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class RequisitionStageConfigConfiguration : IEntityTypeConfiguration<RequisitionStageConfig>
    {
        public void Configure(EntityTypeBuilder<RequisitionStageConfig> builder)
        {
            builder.ToTable("RequisitionStageConfigs");
            builder.HasKey(s => s.RequisitionStageConfigId);

            builder.Property(s => s.StageName)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(s => s.RequisitionId);
            builder.HasIndex(s => new { s.RequisitionId, s.StageOrder }).IsUnique();
        }
    }
}
