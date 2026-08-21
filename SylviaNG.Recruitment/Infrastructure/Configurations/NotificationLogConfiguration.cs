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

            // The HR/candidate notification bell (GetUnreadForAdminHrAsync/GetUnreadCountForAdminHrAsync/
            // GetUnreadForCandidateAsync/GetUnreadCountForCandidateAsync) filters on
            // (RecipientType, IsRead) and sorts by CreatedAt - the index above can't serve that
            // predicate because RecruitmentEvent, its leading column, isn't part of it, so every
            // bell poll (continuous, per logged-in user) seq-scans the whole log table. Partial on
            // IsRead = false so the index only ever holds unread rows and stays small as the log
            // (which never prunes) grows without bound.
            builder.HasIndex(l => new { l.RecipientType, l.CreatedAt })
                .HasFilter("\"IsRead\" = false")
                .HasDatabaseName("IX_NotificationLogs_RecipientType_CreatedAt_Unread");
        }
    }
}
