using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class NotificationTemplateVersionConfiguration : IEntityTypeConfiguration<NotificationTemplateVersion>
    {
        public void Configure(EntityTypeBuilder<NotificationTemplateVersion> builder)
        {
            builder.ToTable("NotificationTemplateVersions");
            builder.HasKey(v => v.NotificationTemplateVersionId);

            builder.Property(v => v.Subject).HasMaxLength(300);
            builder.Property(v => v.Body).IsRequired().HasColumnType("text");

            builder.HasIndex(v => v.NotificationTemplateId);
            builder.HasIndex(v => new { v.NotificationTemplateId, v.VersionNumber }).IsUnique();

            builder.HasOne(v => v.NotificationTemplate)
                .WithMany(t => t.Versions)
                .HasForeignKey(v => v.NotificationTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
