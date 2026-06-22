using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class NotificationEventConfiguration : IEntityTypeConfiguration<NotificationEvent>
    {
        public void Configure(EntityTypeBuilder<NotificationEvent> builder)
        {
            builder.ToTable("NotificationEvents");
            builder.HasKey(e => e.NotificationEventId);

            builder.Property(e => e.EventName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.PipelineStage)
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(e => e.EventName);
            builder.HasIndex(e => e.NotificationTemplateId);
            builder.HasIndex(e => e.RequisitionId);

            // Relationships
            builder.HasOne(e => e.Requisition)
                .WithMany()
                .HasForeignKey(e => e.RequisitionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
