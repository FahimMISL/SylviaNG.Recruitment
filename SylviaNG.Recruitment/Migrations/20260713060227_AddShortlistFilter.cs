using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddShortlistFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortlistFilters",
                columns: table => new
                {
                    ShortlistFilterId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CombineWith = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
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
                    table.PrimaryKey("PK_ShortlistFilters", x => x.ShortlistFilterId);
                });

            migrationBuilder.CreateTable(
                name: "ShortlistFilterCriteria",
                columns: table => new
                {
                    ShortlistFilterCriterionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortlistFilterId = table.Column<long>(type: "bigint", nullable: false),
                    CriterionType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    MinEducationLevel = table.Column<int>(type: "integer", nullable: true),
                    MinExperienceYears = table.Column<int>(type: "integer", nullable: true),
                    RequiredSkillNames = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MinAge = table.Column<int>(type: "integer", nullable: true),
                    MaxAge = table.Column<int>(type: "integer", nullable: true),
                    RequiredDistrict = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MinScreeningScore = table.Column<int>(type: "integer", nullable: true),
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
                    table.PrimaryKey("PK_ShortlistFilterCriteria", x => x.ShortlistFilterCriterionId);
                    table.ForeignKey(
                        name: "FK_ShortlistFilterCriteria_ShortlistFilters_ShortlistFilterId",
                        column: x => x.ShortlistFilterId,
                        principalTable: "ShortlistFilters",
                        principalColumn: "ShortlistFilterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShortlistFilterCriteria_ShortlistFilterId_DisplayOrder",
                table: "ShortlistFilterCriteria",
                columns: new[] { "ShortlistFilterId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ShortlistFilters_Name",
                table: "ShortlistFilters",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortlistFilterCriteria");

            migrationBuilder.DropTable(
                name: "ShortlistFilters");
        }
    }
}
