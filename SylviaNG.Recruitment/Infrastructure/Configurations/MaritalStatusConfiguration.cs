using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class MaritalStatusConfiguration : IEntityTypeConfiguration<MaritalStatus>
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Seeded with the same values the old MaritalStatusEnum carried, so existing behavior is
        // unchanged on migration day - Admin/HR can add/rename/delete freely afterward.
        private static readonly string[] Values = { "Single", "Married", "Other" };

        public void Configure(EntityTypeBuilder<MaritalStatus> builder)
        {
            builder.ToTable("MaritalStatuses");
            builder.HasKey(g => g.MaritalStatusId);

            builder.Property(g => g.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(g => g.Name).IsUnique();

            builder.HasData(Values.Select((name, index) => new
            {
                MaritalStatusId = (long)(index + 1),
                Name = name,
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
