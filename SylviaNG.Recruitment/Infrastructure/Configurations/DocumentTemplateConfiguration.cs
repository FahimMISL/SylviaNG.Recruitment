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

            builder.Property(t => t.TemplateName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.PlaceholdersJson)
                .HasColumnType("text");

            builder.Property(t => t.CurrentVersion)
                .HasDefaultValue(1);

            // Indexes
            builder.HasIndex(t => t.DocumentType);
            builder.HasIndex(t => t.TemplateName);

            // Relationships
            builder.HasMany(t => t.Versions)
                .WithOne(v => v.DocumentTemplate)
                .HasForeignKey(v => v.DocumentTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.GeneratedDocuments)
                .WithOne(g => g.DocumentTemplate)
                .HasForeignKey(g => g.DocumentTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
