using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class LinkCompanyBrandingToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine",
                table: "CompanyBrandings");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "CompanyBrandings");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "CompanyBrandings");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "CompanyBrandings");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "CompanyBrandings");

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "CompanyBrandings",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CompanyBrandings",
                keyColumn: "CompanyBrandingId",
                keyValue: 1L,
                column: "CompanyId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyBrandings_CompanyId",
                table: "CompanyBrandings",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyBrandings_Companies_CompanyId",
                table: "CompanyBrandings",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyBrandings_Companies_CompanyId",
                table: "CompanyBrandings");

            migrationBuilder.DropIndex(
                name: "IX_CompanyBrandings_CompanyId",
                table: "CompanyBrandings");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CompanyBrandings");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine",
                table: "CompanyBrandings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "CompanyBrandings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "CompanyBrandings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "CompanyBrandings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "CompanyBrandings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CompanyBrandings",
                keyColumn: "CompanyBrandingId",
                keyValue: 1L,
                columns: new[] { "AddressLine", "CompanyName", "Email", "Phone", "Website" },
                values: new object[] { "Administrative Building-01, Level-18, Grameen Bank Head Office, Mirpur-2, Dhaka-1216", "Millennium Information Solution Ltd.", "info@mislbd.com", "09601 789 789", null });
        }
    }
}
