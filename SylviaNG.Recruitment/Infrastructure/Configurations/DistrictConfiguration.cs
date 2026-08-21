using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.SeedData;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class DistrictConfiguration : IEntityTypeConfiguration<District>
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<District> builder)
        {
            builder.ToTable("Districts");
            builder.HasKey(d => d.DistrictId);

            builder.Property(d => d.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(d => d.DivisionId);

            builder.HasOne(d => d.Division)
                .WithMany(d => d.Districts)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(BangladeshGeoSeedData.Districts.Select(d => new
            {
                DistrictId = d.Id,
                d.Name,
                d.DivisionId,
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
