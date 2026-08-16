using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyMultiTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Title",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_Departments_Name",
                table: "Departments");

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "UserAccounts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "JobPostings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "JobApplications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "Interviews",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "Departments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LogoFileName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    LogoStoredFileName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    LogoFilePath = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    LogoContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Industry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "CompanyId", "Address", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "Industry", "LogoContentType", "LogoFileName", "LogoFilePath", "LogoStoredFileName", "Name", "Phone", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy", "Website" },
                values: new object[] { 1L, null, new DateTime(2026, 8, 16, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, null, null, null, null, null, null, "Default Company", null, null, "Active", "default_tenant", null, null, null });

            // Data fixup: every pre-existing row (created before multi-tenancy) is backfilled
            // onto the seeded Default Company above, rather than left with a dangling null
            // CompanyId that would make it invisible to any real company under the
            // ICompanyScoped query filter (ApplicationDBContext.OnModelCreating). Departments is
            // deliberately excluded - a null CompanyId there means "shared/global lookup", which
            // is exactly what the pre-existing seeded departments already are.
            migrationBuilder.Sql("UPDATE \"UserAccounts\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"JobPostings\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"JobApplications\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"Interviews\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1L,
                column: "CompanyId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2L,
                column: "CompanyId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3L,
                column: "CompanyId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4L,
                column: "CompanyId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 5L,
                column: "CompanyId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 6L,
                column: "CompanyId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 7L,
                column: "CompanyId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 8L,
                column: "CompanyId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_CompanyId",
                table: "UserAccounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_CompanyId",
                table: "JobPostings",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_CompanyId_Title",
                table: "JobPostings",
                columns: new[] { "CompanyId", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_CompanyId",
                table: "JobApplications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_CompanyId",
                table: "Interviews",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyId_Name",
                table: "Departments",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                table: "Companies",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_Companies_CompanyId",
                table: "UserAccounts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_Companies_CompanyId",
                table: "UserAccounts");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_CompanyId",
                table: "UserAccounts");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_CompanyId",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_CompanyId_Title",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_CompanyId",
                table: "JobApplications");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_CompanyId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Departments_CompanyId_Name",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Departments");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Title",
                table: "JobPostings",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);
        }
    }
}
