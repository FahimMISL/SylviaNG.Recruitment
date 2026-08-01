using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddMajorSubjectLookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MajorSubject",
                table: "CandidateEducations",
                newName: "MajorSubjectOtherText");

            migrationBuilder.AddColumn<long>(
                name: "MajorSubjectSscHscId",
                table: "CandidateEducations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MajorSubjectUniversityId",
                table: "CandidateEducations",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MajorSubjectsSscHsc",
                columns: table => new
                {
                    MajorSubjectSscHscId = table.Column<long>(type: "bigint", nullable: false)
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
                    table.PrimaryKey("PK_MajorSubjectsSscHsc", x => x.MajorSubjectSscHscId);
                });

            migrationBuilder.CreateTable(
                name: "MajorSubjectsUniversity",
                columns: table => new
                {
                    MajorSubjectUniversityId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
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
                    table.PrimaryKey("PK_MajorSubjectsUniversity", x => x.MajorSubjectUniversityId);
                });

            migrationBuilder.InsertData(
                table: "MajorSubjectsSscHsc",
                columns: new[] { "MajorSubjectSscHscId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Science", null, 1, "default_tenant", null, null },
                    { 2L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Arts", null, 1, "default_tenant", null, null },
                    { 3L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Humanities", null, 1, "default_tenant", null, null },
                    { 4L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Commerce", null, 1, "default_tenant", null, null },
                    { 5L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Business Studies", null, 1, "default_tenant", null, null },
                    { 6L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Technical", null, 1, "default_tenant", null, null },
                    { 7L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Vocational", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.InsertData(
                table: "MajorSubjectsUniversity",
                columns: new[] { "MajorSubjectUniversityId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Computer Science & Engineering", null, 1, "default_tenant", null, null },
                    { 2L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Business Administration", null, 1, "default_tenant", null, null },
                    { 3L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Accounting", null, 1, "default_tenant", null, null },
                    { 4L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Finance", null, 1, "default_tenant", null, null },
                    { 5L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Marketing", null, 1, "default_tenant", null, null },
                    { 6L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Management", null, 1, "default_tenant", null, null },
                    { 7L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Economics", null, 1, "default_tenant", null, null },
                    { 8L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "English", null, 1, "default_tenant", null, null },
                    { 9L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Law", null, 1, "default_tenant", null, null },
                    { 10L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Electrical & Electronic Engineering", null, 1, "default_tenant", null, null },
                    { 11L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Civil Engineering", null, 1, "default_tenant", null, null },
                    { 12L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Mechanical Engineering", null, 1, "default_tenant", null, null },
                    { 13L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Pharmacy", null, 1, "default_tenant", null, null },
                    { 14L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Medicine", null, 1, "default_tenant", null, null },
                    { 15L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Architecture", null, 1, "default_tenant", null, null },
                    { 16L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Mathematics", null, 1, "default_tenant", null, null },
                    { 17L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Statistics", null, 1, "default_tenant", null, null },
                    { 18L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Psychology", null, 1, "default_tenant", null, null },
                    { 19L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Sociology", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEducations_MajorSubjectSscHscId",
                table: "CandidateEducations",
                column: "MajorSubjectSscHscId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEducations_MajorSubjectUniversityId",
                table: "CandidateEducations",
                column: "MajorSubjectUniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_MajorSubjectsSscHsc_Name",
                table: "MajorSubjectsSscHsc",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MajorSubjectsUniversity_Name",
                table: "MajorSubjectsUniversity",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateEducations_MajorSubjectsSscHsc_MajorSubjectSscHscId",
                table: "CandidateEducations",
                column: "MajorSubjectSscHscId",
                principalTable: "MajorSubjectsSscHsc",
                principalColumn: "MajorSubjectSscHscId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateEducations_MajorSubjectsUniversity_MajorSubjectUni~",
                table: "CandidateEducations",
                column: "MajorSubjectUniversityId",
                principalTable: "MajorSubjectsUniversity",
                principalColumn: "MajorSubjectUniversityId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateEducations_MajorSubjectsSscHsc_MajorSubjectSscHscId",
                table: "CandidateEducations");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateEducations_MajorSubjectsUniversity_MajorSubjectUni~",
                table: "CandidateEducations");

            migrationBuilder.DropTable(
                name: "MajorSubjectsSscHsc");

            migrationBuilder.DropTable(
                name: "MajorSubjectsUniversity");

            migrationBuilder.DropIndex(
                name: "IX_CandidateEducations_MajorSubjectSscHscId",
                table: "CandidateEducations");

            migrationBuilder.DropIndex(
                name: "IX_CandidateEducations_MajorSubjectUniversityId",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "MajorSubjectSscHscId",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "MajorSubjectUniversityId",
                table: "CandidateEducations");

            migrationBuilder.RenameColumn(
                name: "MajorSubjectOtherText",
                table: "CandidateEducations",
                newName: "MajorSubject");
        }
    }
}
