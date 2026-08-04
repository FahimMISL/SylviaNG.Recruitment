using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CompanyBrandingConfiguration : IEntityTypeConfiguration<CompanyBranding>
    {
        private static readonly DateTime SeedCreatedAt = new(2026, 7, 27, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<CompanyBranding> builder)
        {
            builder.ToTable("CompanyBrandings");
            builder.HasKey(b => b.CompanyBrandingId);

            builder.Property(b => b.LogoFileName).HasMaxLength(300);
            builder.Property(b => b.LogoStoredFileName).HasMaxLength(300);
            builder.Property(b => b.LogoFilePath).HasMaxLength(300);
            builder.Property(b => b.LogoContentType).HasMaxLength(100);

            builder.Property(b => b.CompanyName).HasMaxLength(200);
            builder.Property(b => b.AddressLine).HasMaxLength(200);
            builder.Property(b => b.Phone).HasMaxLength(100);
            builder.Property(b => b.Email).HasMaxLength(100);
            builder.Property(b => b.Website).HasMaxLength(200);

            builder.Property(b => b.PrimaryColor).HasMaxLength(20);
            builder.Property(b => b.SecondaryColor).HasMaxLength(20);
            builder.Property(b => b.AccentColor).HasMaxLength(20);
            builder.Property(b => b.FontFamily).HasMaxLength(100);

            builder.Property(b => b.HeaderLayout).HasConversion<string>().HasMaxLength(50);
            builder.Property(b => b.FooterLayout).HasConversion<string>().HasMaxLength(50);
            builder.Property(b => b.BorderStyle).HasConversion<string>().HasMaxLength(50);
            builder.Property(b => b.HeaderDividerStyle).HasConversion<string>().HasMaxLength(50);
            builder.Property(b => b.FooterDividerStyle).HasConversion<string>().HasMaxLength(50);
            builder.Property(b => b.QrCodePosition).HasConversion<string>().HasMaxLength(50);
            builder.Property(b => b.SignaturePosition).HasConversion<string>().HasMaxLength(50);
            builder.Property(b => b.SealPosition).HasConversion<string>().HasMaxLength(50);

            builder.Property(b => b.DocumentReferenceFormat).IsRequired().HasMaxLength(200);

            // Single MISL row seeded for now - Audit.TenantId already scopes it per tenant for
            // later SaaS use, same singleton-row convention as ApplicationSettingConfiguration.
            builder.HasData(new
            {
                CompanyBrandingId = 1L,
                LogoFileName = (string?)null,
                LogoStoredFileName = (string?)null,
                LogoFilePath = (string?)null,
                LogoContentType = (string?)null,
                CompanyName = "Millennium Information Solution Ltd.",
                AddressLine = "Administrative Building-01, Level-18, Grameen Bank Head Office, Mirpur-2, Dhaka-1216",
                Phone = "09601 789 789",
                Email = "info@mislbd.com",
                Website = (string?)null,
                PrimaryColor = "#7A2E2E",
                SecondaryColor = "#1F2937",
                AccentColor = "#DC2626",
                FontFamily = "Helvetica",
                HeaderLayout = HeaderLayoutEnum.LogoLeftTextRight,
                FooterLayout = FooterLayoutEnum.ThreeColumn,
                MarginTop = 30,
                MarginBottom = 30,
                MarginLeft = 30,
                MarginRight = 30,
                BorderStyle = DocumentBorderStyleEnum.Solid,
                CornerRadius = 6,
                BackgroundWatermarkEnabled = false,
                WatermarkOpacity = 0,
                HeaderDividerStyle = DocumentDividerStyleEnum.SolidLine,
                FooterDividerStyle = DocumentDividerStyleEnum.SolidLine,
                QrCodePosition = DocumentElementPositionEnum.TopRight,
                SignaturePosition = DocumentElementPositionEnum.BottomLeft,
                SealPosition = DocumentElementPositionEnum.BottomRight,
                DocumentReferenceFormat = "{ORG}/{DOCTYPE}/{YEAR}/{SEQ}",
                ShowPageNumbers = true,
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
