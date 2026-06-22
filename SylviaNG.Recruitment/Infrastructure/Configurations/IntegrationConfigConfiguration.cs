using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class IntegrationConfigConfiguration : IEntityTypeConfiguration<IntegrationConfig>
    {
        public void Configure(EntityTypeBuilder<IntegrationConfig> builder)
        {
            builder.ToTable("IntegrationConfigs");
            builder.HasKey(c => c.IntegrationConfigId);

            builder.Property(c => c.IntegrationType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(c => c.ConfigName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.BaseUrl)
                .HasMaxLength(500);

            builder.Property(c => c.ApiKeyEncrypted)
                .HasMaxLength(500);

            builder.Property(c => c.AdditionalSettingsJson)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(c => c.IntegrationType);
            builder.HasIndex(c => c.ConfigName).IsUnique();
        }
    }
}
