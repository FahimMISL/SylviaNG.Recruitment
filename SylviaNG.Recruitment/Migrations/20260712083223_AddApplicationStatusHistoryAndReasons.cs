using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationStatusHistoryAndReasons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationStatusReasons",
                columns: table => new
                {
                    ApplicationStatusReasonId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AppliesToStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_ApplicationStatusReasons", x => x.ApplicationStatusReasonId);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationStatusHistories",
                columns: table => new
                {
                    ApplicationStatusHistoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    FromStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ToStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ChangedByUserName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReasonId = table.Column<long>(type: "bigint", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_ApplicationStatusHistories", x => x.ApplicationStatusHistoryId);
                    table.ForeignKey(
                        name: "FK_ApplicationStatusHistories_ApplicationStatusReasons_ReasonId",
                        column: x => x.ReasonId,
                        principalTable: "ApplicationStatusReasons",
                        principalColumn: "ApplicationStatusReasonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationStatusHistories_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "JobApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStatusHistories_JobApplicationId",
                table: "ApplicationStatusHistories",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStatusHistories_ReasonId",
                table: "ApplicationStatusHistories",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStatusReasons_AppliesToStatus",
                table: "ApplicationStatusReasons",
                column: "AppliesToStatus");

            // US-036 AC3: seed default reject/withdraw reasons for the dropdown.
            migrationBuilder.InsertData(
                table: "ApplicationStatusReasons",
                columns: new[] { "Label", "AppliesToStatus", "DisplayOrder", "IsActive", "TenantId", "Status" },
                values: new object[,]
                {
                    { "Not enough experience", "Rejected", 1, true, "default_tenant", 0 },
                    { "Skills mismatch", "Rejected", 2, true, "default_tenant", 0 },
                    { "Failed screening", "Rejected", 3, true, "default_tenant", 0 },
                    { "Position filled internally", "Rejected", 4, true, "default_tenant", 0 },
                    { "Candidate unresponsive", "Rejected", 5, true, "default_tenant", 0 },
                    { "Candidate withdrew - accepted another offer", "Withdrawn", 1, true, "default_tenant", 0 },
                    { "Candidate withdrew - salary expectations", "Withdrawn", 2, true, "default_tenant", 0 },
                    { "Candidate withdrew - no longer interested", "Withdrawn", 3, true, "default_tenant", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationStatusHistories");

            migrationBuilder.DropTable(
                name: "ApplicationStatusReasons");
        }
    }
}
