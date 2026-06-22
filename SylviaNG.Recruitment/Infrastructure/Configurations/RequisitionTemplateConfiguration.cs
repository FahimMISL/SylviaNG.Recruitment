using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class RequisitionTemplateConfiguration : IEntityTypeConfiguration<RequisitionTemplate>
    {
        public void Configure(EntityTypeBuilder<RequisitionTemplate> builder)
        {
            builder.ToTable("RequisitionTemplates");
            builder.HasKey(t => t.RequisitionTemplateId);

            builder.Property(t => t.TemplateName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.StageConfigJson)
                .IsRequired()
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(t => t.TemplateName).IsUnique();
        }
    }
}
