using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class DocumentTemplateVersionConfiguration : IEntityTypeConfiguration<DocumentTemplateVersion>
    {
        public void Configure(EntityTypeBuilder<DocumentTemplateVersion> builder)
        {
            builder.ToTable("DocumentTemplateVersions");
            builder.HasKey(v => v.DocumentTemplateVersionId);

            builder.Property(v => v.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(v => v.ChangeNotes)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(v => v.DocumentTemplateId);
            builder.HasIndex(v => new { v.DocumentTemplateId, v.VersionNumber }).IsUnique();
        }
    }
}
