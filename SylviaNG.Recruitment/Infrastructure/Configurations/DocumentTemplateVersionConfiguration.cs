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

            builder.Property(v => v.Body).IsRequired().HasColumnType("text");

            builder.HasIndex(v => v.DocumentTemplateId);
            builder.HasIndex(v => new { v.DocumentTemplateId, v.VersionNumber }).IsUnique();

            builder.HasOne(v => v.DocumentTemplate)
                .WithMany(t => t.Versions)
                .HasForeignKey(v => v.DocumentTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
