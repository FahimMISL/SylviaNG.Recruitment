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

            builder.Property(l => l.Channel)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(l => l.Recipient)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Subject)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(l => l.Body)
                .HasColumnType("text");

            builder.Property(l => l.DeliveryStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(l => l.FailureReason)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(l => l.NotificationEventId);
            builder.HasIndex(l => l.CandidateId);
            builder.HasIndex(l => l.DeliveryStatus);

            // Relationships
            builder.HasOne(l => l.NotificationEvent)
                .WithMany()
                .HasForeignKey(l => l.NotificationEventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Candidate)
                .WithMany()
                .HasForeignKey(l => l.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.JobApplication)
                .WithMany()
                .HasForeignKey(l => l.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
