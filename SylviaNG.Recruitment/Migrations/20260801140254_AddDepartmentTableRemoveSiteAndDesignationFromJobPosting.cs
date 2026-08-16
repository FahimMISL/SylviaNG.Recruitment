using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentTableRemoveSiteAndDesignationFromJobPosting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobPostings_SiteId",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_SiteId_Title",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "DesignationId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "JobPostings");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Engineering", null, 1, "default_tenant", null, null },
                    { 2L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Business & Sales", null, 1, "default_tenant", null, null },
                    { 3L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Human Resources", null, 1, "default_tenant", null, null },
                    { 4L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Finance", null, 1, "default_tenant", null, null },
                    { 5L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Marketing", null, 1, "default_tenant", null, null },
                    { 6L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Operations", null, 1, "default_tenant", null, null },
                    { 7L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Customer Support", null, 1, "default_tenant", null, null },
                    { 8L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Legal & Compliance", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.UpdateData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 2L,
                column: "DepartmentId",
                value: 1L);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_DepartmentId",
                table: "JobPostings",
                column: "DepartmentId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_Departments_DepartmentId",
                table: "JobPostings",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_Departments_DepartmentId",
                table: "JobPostings");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_DepartmentId",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_Title",
                table: "JobPostings");

            migrationBuilder.AddColumn<long>(
                name: "DesignationId",
                table: "JobPostings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SiteId",
                table: "JobPostings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 1L,
                columns: new[] { "DesignationId", "SiteId" },
                values: new object[] { 1L, 1L });

            migrationBuilder.UpdateData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 2L,
                columns: new[] { "DepartmentId", "DesignationId", "SiteId" },
                values: new object[] { 2L, 2L, 1L });

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_SiteId",
                table: "JobPostings",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_SiteId_Title",
                table: "JobPostings",
                columns: new[] { "SiteId", "Title" },
                unique: true);
        }
    }
}
