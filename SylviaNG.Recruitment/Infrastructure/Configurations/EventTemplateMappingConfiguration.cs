using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class EventTemplateMappingConfiguration : IEntityTypeConfiguration<EventTemplateMapping>
    {
        public void Configure(EntityTypeBuilder<EventTemplateMapping> builder)
        {
            builder.ToTable("EventTemplateMappings");
            builder.HasKey(m => m.EventTemplateMappingId);

            builder.Property(m => m.RecruitmentEvent)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(m => m.Channel)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(m => m.RecipientType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasIndex(m => new { m.RecruitmentEvent, m.Channel, m.RecipientType }).IsUnique();

            builder.HasOne(m => m.NotificationTemplate)
                .WithMany(t => t.EventTemplateMappings)
                .HasForeignKey(m => m.NotificationTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
