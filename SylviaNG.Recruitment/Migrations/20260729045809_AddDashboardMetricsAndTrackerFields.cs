using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardMetricsAndTrackerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresManualApproval",
                table: "JobApplicationStageProgresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SlaDaysSnapshot",
                table: "JobApplicationStageProgresses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StageEnteredAt",
                table: "JobApplicationStageProgresses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultStaleDaysThreshold",
                table: "ApplicationSettings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DashboardWidgetConfigs",
                columns: table => new
                {
                    DashboardWidgetConfigId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WidgetKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsVisibleForAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    IsVisibleForHR = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_DashboardWidgetConfigs", x => x.DashboardWidgetConfigId);
                });

            migrationBuilder.UpdateData(
                table: "ApplicationSettings",
                keyColumn: "ApplicationSettingId",
                keyValue: 1L,
                column: "DefaultStaleDaysThreshold",
                value: null);

            migrationBuilder.InsertData(
                table: "DashboardWidgetConfigs",
                columns: new[] { "DashboardWidgetConfigId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsVisibleForAdmin", "IsVisibleForHR", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy", "WidgetKey" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, true, true, null, 1, "default_tenant", null, null, "OpenVacancies" },
                    { 2L, new DateTime(2026, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, true, true, null, 1, "default_tenant", null, null, "TotalApplications" },
                    { 3L, new DateTime(2026, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, true, true, null, 1, "default_tenant", null, null, "UpcomingInterviews" },
                    { 4L, new DateTime(2026, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, true, true, null, 1, "default_tenant", null, null, "PendingApprovals" },
                    { 5L, new DateTime(2026, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, true, true, null, 1, "default_tenant", null, null, "OffersPendingAcceptance" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardWidgetConfigs_WidgetKey",
                table: "DashboardWidgetConfigs",
                column: "WidgetKey",
                unique: true);

            // Best-effort backfill for existing rows (EP-14 US-105/US-109): snapshot the current
            // PipelineStage's ManualApprovalRequired/SlaDays onto every existing progress row -
            // won't be accurate for pipelines already edited since a row was first provisioned
            // (same limitation the existing StageName/DisplayOrder snapshot has always had), but
            // matches "current" for anything unedited.
            migrationBuilder.Sql(
                @"UPDATE ""JobApplicationStageProgresses"" AS jsp
                  SET ""RequiresManualApproval"" = ps.""ManualApprovalRequired"",
                      ""SlaDaysSnapshot"" = ps.""SlaDays""
                  FROM ""PipelineStages"" AS ps
                  WHERE jsp.""PipelineStageId"" = ps.""PipelineStageId"";");

            // Stamp StageEnteredAt = now for every currently-InProgress row, so existing in-flight
            // applications start their "Days in Current Stage" counter at 0 on launch day instead
            // of showing blank/"Unknown" for every pre-existing application (deliberate MVP/demo
            // tradeoff - see Doc/features/EP-14-F1-overview-dashboard-and-tracker.md).
            migrationBuilder.Sql(
                @"UPDATE ""JobApplicationStageProgresses""
                  SET ""StageEnteredAt"" = NOW() AT TIME ZONE 'UTC'
                  WHERE ""Status"" = 'InProgress' AND ""StageEnteredAt"" IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DashboardWidgetConfigs");

            migrationBuilder.DropColumn(
                name: "RequiresManualApproval",
                table: "JobApplicationStageProgresses");

            migrationBuilder.DropColumn(
                name: "SlaDaysSnapshot",
                table: "JobApplicationStageProgresses");

            migrationBuilder.DropColumn(
                name: "StageEnteredAt",
                table: "JobApplicationStageProgresses");

            migrationBuilder.DropColumn(
                name: "DefaultStaleDaysThreshold",
                table: "ApplicationSettings");
        }
    }
}
