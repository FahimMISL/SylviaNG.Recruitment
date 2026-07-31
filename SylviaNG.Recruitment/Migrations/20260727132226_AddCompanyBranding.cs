using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyBranding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyBrandings",
                columns: table => new
                {
                    CompanyBrandingId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogoFileName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    LogoStoredFileName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    LogoFilePath = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    LogoContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AddressLine = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PrimaryColor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SecondaryColor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AccentColor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FontFamily = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    HeaderLayout = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FooterLayout = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MarginTop = table.Column<int>(type: "integer", nullable: false),
                    MarginBottom = table.Column<int>(type: "integer", nullable: false),
                    MarginLeft = table.Column<int>(type: "integer", nullable: false),
                    MarginRight = table.Column<int>(type: "integer", nullable: false),
                    BorderStyle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CornerRadius = table.Column<int>(type: "integer", nullable: false),
                    BackgroundWatermarkEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    WatermarkOpacity = table.Column<int>(type: "integer", nullable: false),
                    HeaderDividerStyle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FooterDividerStyle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    QrCodePosition = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SignaturePosition = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SealPosition = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DocumentReferenceFormat = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ShowPageNumbers = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyBrandings", x => x.CompanyBrandingId);
                });

            migrationBuilder.InsertData(
                table: "CompanyBrandings",
                columns: new[] { "CompanyBrandingId", "AccentColor", "AddressLine", "BackgroundWatermarkEnabled", "BorderStyle", "CompanyName", "CornerRadius", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DocumentReferenceFormat", "Email", "FontFamily", "FooterDividerStyle", "FooterLayout", "HeaderDividerStyle", "HeaderLayout", "LogoContentType", "LogoFileName", "LogoFilePath", "LogoStoredFileName", "MarginBottom", "MarginLeft", "MarginRight", "MarginTop", "Phone", "PrimaryColor", "QrCodePosition", "Remarks", "SealPosition", "SecondaryColor", "ShowPageNumbers", "SignaturePosition", "Status", "TenantId", "UpdatedAt", "UpdatedBy", "WatermarkOpacity", "Website" },
                values: new object[] { 1L, "#DC2626", "Administrative Building-01, Level-18, Grameen Bank Head Office, Mirpur-2, Dhaka-1216", false, "Solid", "Millennium Information Solution Ltd.", 6, new DateTime(2026, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "{ORG}/{DOCTYPE}/{YEAR}/{SEQ}", "info@mislbd.com", "Helvetica", "SolidLine", "ThreeColumn", "SolidLine", "LogoLeftTextRight", null, null, null, null, 30, 30, 30, 30, "09601 789 789", "#7A2E2E", "TopRight", null, "BottomRight", "#1F2937", true, "BottomLeft", 1, "default_tenant", null, null, 0, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyBrandings");
        }
    }
}
