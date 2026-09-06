using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class BloodGroupConfiguration : IEntityTypeConfiguration<BloodGroup>
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Seeded with the same values the old BloodGroupEnum carried, so existing behavior is
        // unchanged on migration day - Admin/HR can add/rename/delete freely afterward.
        private static readonly string[] Values = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };

        public void Configure(EntityTypeBuilder<BloodGroup> builder)
        {
            builder.ToTable("BloodGroups");
            builder.HasKey(g => g.BloodGroupId);

            builder.Property(g => g.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(g => g.Name).IsUnique();

            builder.HasData(Values.Select((name, index) => new
            {
                BloodGroupId = (long)(index + 1),
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
