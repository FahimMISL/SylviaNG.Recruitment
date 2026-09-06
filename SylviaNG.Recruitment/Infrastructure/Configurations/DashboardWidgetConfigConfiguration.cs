using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Application.Features.Dashboard.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class DashboardWidgetConfigConfiguration : IEntityTypeConfiguration<DashboardWidgetConfig>
    {
        private static readonly DateTime SeedCreatedAt = new(2026, 7, 29, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<DashboardWidgetConfig> builder)
        {
            builder.ToTable("DashboardWidgetConfigs");
            builder.HasKey(w => w.DashboardWidgetConfigId);

            builder.Property(w => w.WidgetKey).IsRequired().HasMaxLength(50);
            builder.HasIndex(w => w.WidgetKey).IsUnique();

            builder.HasData(DashboardWidgetKeys.All.Select((key, index) => new
            {
                DashboardWidgetConfigId = (long)(index + 1),
                WidgetKey = key,
                IsVisibleForAdmin = true,
                IsVisibleForHR = true,
                TenantId = "default_tenant",
                Remarks = (string?)null,
                CreatedAt = SeedCreatedAt,
                CreatedBy = 1L,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (long?)null,
                DeletedAt = (DateTime?)null,
                DeletedBy = (long?)null,
                Status = 1
            }));
        }
    }
}
