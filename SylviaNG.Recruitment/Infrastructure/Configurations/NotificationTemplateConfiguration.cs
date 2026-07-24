using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
    {
        public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
        {
            builder.ToTable("NotificationTemplates");
            builder.HasKey(t => t.NotificationTemplateId);

            builder.Property(t => t.Channel)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(t => t.Code).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Subject).HasMaxLength(300);
            builder.Property(t => t.Body).IsRequired().HasColumnType("text");

            builder.HasIndex(t => t.Code).IsUnique();
        }
    }
}
