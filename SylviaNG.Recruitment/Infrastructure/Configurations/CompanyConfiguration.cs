using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        private static readonly DateTime SeedCreatedAt = new(2026, 8, 16, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");
            builder.HasKey(c => c.CompanyId);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.HasIndex(c => c.Name).IsUnique();

            builder.Property(c => c.ExternalId).HasMaxLength(200);
            builder.HasIndex(c => c.ExternalId).IsUnique();

            builder.Property(c => c.LogoFileName).HasMaxLength(300);
            builder.Property(c => c.LogoStoredFileName).HasMaxLength(300);
            builder.Property(c => c.LogoFilePath).HasMaxLength(300);
            builder.Property(c => c.LogoContentType).HasMaxLength(100);

            builder.Property(c => c.Email).HasMaxLength(200);
            builder.Property(c => c.Phone).HasMaxLength(50);
            builder.Property(c => c.Address).HasMaxLength(500);
            builder.Property(c => c.Website).HasMaxLength(200);
            builder.Property(c => c.Industry).HasMaxLength(100);

            builder.Property(c => c.TradeLicenseNumber).HasMaxLength(100);
            builder.Property(c => c.BinNumber).HasMaxLength(100);

            builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

            builder.HasMany(c => c.UserAccounts)
                .WithOne(u => u.Company)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Migration-time seed: every pre-existing UserAccount/JobPosting/JobApplication/
            // Interview row created before multi-tenancy is backfilled onto this single company
            // rather than left with a dangling null CompanyId (see AddCompanyMultiTenancy
            // migration's data-fixup step).
            builder.HasData(new
            {
                CompanyId = 1L,
                ExternalId = (string?)null,
                Name = "Default Company",
                LogoFileName = (string?)null,
                LogoStoredFileName = (string?)null,
                LogoFilePath = (string?)null,
                LogoContentType = (string?)null,
                Email = (string?)null,
                Phone = (string?)null,
                Address = (string?)null,
                Website = (string?)null,
                Industry = (string?)null,
                TradeLicenseNumber = (string?)null,
                BinNumber = (string?)null,
                Status = Domain.Enums.CompanyStatusEnum.Active,
                TenantId = "default_tenant",
                Remarks = (string?)null,
                CreatedAt = SeedCreatedAt,
                CreatedBy = 1L,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (long?)null,
                DeletedAt = (DateTime?)null,
                DeletedBy = (long?)null,
            });
        }
    }
}
