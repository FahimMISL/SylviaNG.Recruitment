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

            builder.Property(e => e.ExportType).HasConversion<string>().HasMaxLength(50);
            builder.Property(e => e.Format).HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);

            builder.Property(e => e.RequestedByUserName).HasMaxLength(200);
            builder.Property(e => e.RequestedByEmail).HasMaxLength(320);
            builder.Property(e => e.FileName).HasMaxLength(300);
            builder.Property(e => e.ContentObjectKey).HasMaxLength(500);
            builder.Property(e => e.ContentType).HasMaxLength(150);
            builder.Property(e => e.FailureReason).HasMaxLength(1000);

            builder.HasIndex(e => e.Status);
            builder.HasIndex(e => e.ExpiresAt);
            builder.HasIndex(e => e.CompanyId);
        }
    }
}
