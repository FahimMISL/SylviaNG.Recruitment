using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        private static readonly DateTime SeedCreatedAt = new(2026, 7, 30, 0, 0, 0, DateTimeKind.Utc);

        // Mirror rows for the 5 fixed UserRoleEnum system roles (EP-15/US-112), so they appear
        // alongside custom roles in the management UI. Their Permissions are informational only -
        // system roles keep enforcing via [Authorize(Roles=...)], not this table.
        private static readonly string[] SystemRoleNames = Enum.GetNames(typeof(UserRoleEnum));

        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(r => r.RoleId);

            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(r => r.Name).IsUnique();

            builder.HasData(SystemRoleNames.Select((name, index) => new
            {
                RoleId = (long)(index + 1),
                Name = name,
                IsSystemRole = true,
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
