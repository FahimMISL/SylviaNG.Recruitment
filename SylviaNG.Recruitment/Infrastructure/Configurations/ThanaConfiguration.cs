using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.SeedData;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ThanaConfiguration : IEntityTypeConfiguration<Thana>
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<Thana> builder)
        {
            builder.ToTable("Thanas");
            builder.HasKey(t => t.ThanaId);

            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(t => t.DistrictId);

            builder.HasOne(t => t.District)
                .WithMany(d => d.Thanas)
                .HasForeignKey(t => t.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(BangladeshGeoSeedData.Thanas.Select(t => new
            {
                ThanaId = t.Id,
                t.Name,
                t.DistrictId,
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
