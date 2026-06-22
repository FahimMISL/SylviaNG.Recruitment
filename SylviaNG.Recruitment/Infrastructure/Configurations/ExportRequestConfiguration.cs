using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExportRequestConfiguration : IEntityTypeConfiguration<ExportRequest>
    {
        public void Configure(EntityTypeBuilder<ExportRequest> builder)
        {
            builder.ToTable("ExportRequests");
            builder.HasKey(e => e.ExportRequestId);

            builder.Property(e => e.ExportType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.FilterCriteriaJson)
                .HasColumnType("text");

            builder.Property(e => e.ExportStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.FileUrl)
                .HasMaxLength(500);

            builder.Property(e => e.FileName)
                .HasMaxLength(300);

            builder.Property(e => e.FileFormat)
                .HasMaxLength(50);

            builder.Property(e => e.FailureReason)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(e => e.RequestedByUserId);
            builder.HasIndex(e => e.ExportStatus);

            // Relationships
            builder.HasOne(e => e.RequestedByUser)
                .WithMany()
                .HasForeignKey(e => e.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
