using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class DocumentTemplateConfiguration : IEntityTypeConfiguration<DocumentTemplate>
    {
        public void Configure(EntityTypeBuilder<DocumentTemplate> builder)
        {
            builder.ToTable("DocumentTemplates");
            builder.HasKey(t => t.DocumentTemplateId);

            builder.Property(t => t.DocumentType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(t => t.Code).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Body).IsRequired().HasColumnType("text");

            builder.HasIndex(t => t.Code).IsUnique();
        }
    }
}
