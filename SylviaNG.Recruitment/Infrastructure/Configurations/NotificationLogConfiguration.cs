using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
    {
        public void Configure(EntityTypeBuilder<NotificationLog> builder)
        {
            builder.ToTable("NotificationLogs");
            builder.HasKey(l => l.NotificationLogId);

            builder.Property(l => l.RecruitmentEvent).HasConversion<string>().HasMaxLength(50);
            builder.Property(l => l.Channel).HasConversion<string>().HasMaxLength(50);
            builder.Property(l => l.RecipientType).HasConversion<string>().HasMaxLength(50);
            builder.Property(l => l.DeliveryStatus).HasConversion<string>().HasMaxLength(50);

            builder.Property(l => l.RecipientAddress).IsRequired().HasMaxLength(320);
            builder.Property(l => l.RenderedSubject).HasMaxLength(300);
            builder.Property(l => l.FailureReason).HasMaxLength(1000);

            builder.HasOne(l => l.NotificationTemplate)
                .WithMany()
                .HasForeignKey(l => l.NotificationTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.JobApplication)
                .WithMany()
                .HasForeignKey(l => l.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(l => new { l.RecruitmentEvent, l.RecipientType, l.CreatedAt });
            builder.HasIndex(l => l.JobApplicationId);
        }
    }
}
