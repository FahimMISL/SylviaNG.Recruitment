using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhase1_ProfileFieldConfigs_EligibilityFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxAge",
                table: "JobPostings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinAge",
                table: "JobPostings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MinEducationLevel",
                table: "JobPostings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinExperienceYears",
                table: "JobPostings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredDistrict",
                table: "JobPostings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProfileFieldConfigs",
                columns: table => new
                {
                    ProfileFieldConfigId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FieldName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FieldType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    OptionsJson = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_ProfileFieldConfigs", x => x.ProfileFieldConfigId);
                });

            migrationBuilder.UpdateData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 1L,
                columns: new[] { "MaxAge", "MinAge", "MinEducationLevel", "MinExperienceYears", "RequiredDistrict" },
                values: new object[] { null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 2L,
                columns: new[] { "MaxAge", "MinAge", "MinEducationLevel", "MinExperienceYears", "RequiredDistrict" },
                values: new object[] { null, null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileFieldConfigs_FieldName",
                table: "ProfileFieldConfigs",
                column: "FieldName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfileFieldConfigs");

            migrationBuilder.DropColumn(
                name: "MaxAge",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "MinAge",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "MinEducationLevel",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "MinExperienceYears",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "RequiredDistrict",
                table: "JobPostings");
        }
    }
}
