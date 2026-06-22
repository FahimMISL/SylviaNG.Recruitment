using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ProfileFieldConfigConfiguration : IEntityTypeConfiguration<ProfileFieldConfig>
    {
        public void Configure(EntityTypeBuilder<ProfileFieldConfig> builder)
        {
            builder.ToTable("ProfileFieldConfigs");
            builder.HasKey(p => p.ProfileFieldConfigId);

            builder.Property(p => p.FieldName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Label)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.FieldType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.OptionsJson)
                .HasMaxLength(2000);

            builder.HasIndex(p => p.FieldName).IsUnique();
        }
    }
}
