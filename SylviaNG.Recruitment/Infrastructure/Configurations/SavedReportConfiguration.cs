using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class SavedReportConfiguration : IEntityTypeConfiguration<SavedReport>
    {
        public void Configure(EntityTypeBuilder<SavedReport> builder)
        {
            builder.ToTable("SavedReports");
            builder.HasKey(r => r.SavedReportId);

            builder.Property(r => r.ReportName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Description)
                .HasMaxLength(500);

            builder.Property(r => r.ReportType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.FilterCriteriaJson)
                .HasColumnType("text");

            builder.Property(r => r.ColumnConfigJson)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(r => r.CreatedByUserId);

            // Relationships
            builder.HasOne(r => r.CreatedByUser)
                .WithMany()
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
