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

            builder.Property(t => t.TemplateName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Channel)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(t => t.Body)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(t => t.PlaceholdersJson)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(t => t.TemplateName);
            builder.HasIndex(t => t.Channel);

            // Relationships
            builder.HasMany(t => t.NotificationEvents)
                .WithOne(e => e.NotificationTemplate)
                .HasForeignKey(e => e.NotificationTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
