using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.SeedData;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class DivisionConfiguration : IEntityTypeConfiguration<Division>
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<Division> builder)
        {
            builder.ToTable("Divisions");
            builder.HasKey(d => d.DivisionId);

            builder.Property(d => d.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(d => d.Name).IsUnique();

            builder.HasData(BangladeshGeoSeedData.Divisions.Select(d => new
            {
                DivisionId = d.Id,
                d.Name,
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
