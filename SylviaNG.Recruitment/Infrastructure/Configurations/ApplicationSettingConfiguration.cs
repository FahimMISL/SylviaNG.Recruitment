using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ApplicationSettingConfiguration : IEntityTypeConfiguration<ApplicationSetting>
    {
        private static readonly DateTime SeedCreatedAt = new(2026, 7, 17, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<ApplicationSetting> builder)
        {
            builder.ToTable("ApplicationSettings");
            builder.HasKey(s => s.ApplicationSettingId);

            // Gate is off (0) until Admin explicitly sets a threshold - avoids retroactively
            // blocking existing candidates who never anticipated a completeness requirement.
            builder.HasData(new
            {
                ApplicationSettingId = 1L,
                MinimumProfileCompletenessPercentage = 0,
                TenantId = "default_tenant",
                Remarks = (string?)null,
                CreatedAt = SeedCreatedAt,
                CreatedBy = 1L,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (long?)null,
                DeletedAt = (DateTime?)null,
                DeletedBy = (long?)null,
                Status = 1
            });
        }
    }
}
