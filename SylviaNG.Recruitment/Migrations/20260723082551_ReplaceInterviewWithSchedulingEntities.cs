using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceInterviewWithSchedulingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "Result",
                table: "Interviews");

            migrationBuilder.RenameColumn(
                name: "ScheduledDate",
                table: "Interviews",
                newName: "SmsLoggedAt");

            migrationBuilder.RenameColumn(
                name: "InterviewerId",
                table: "Interviews",
                newName: "PipelineStageId");

            migrationBuilder.RenameColumn(
                name: "Feedback",
                table: "Interviews",
                newName: "Notes");

            migrationBuilder.Sql(@"ALTER TABLE ""Interviews"" ALTER COLUMN ""Round"" TYPE integer USING (CASE WHEN ""Round"" ~ '^[0-9]+$' THEN ""Round""::integer ELSE 0 END);");

            migrationBuilder.AlterColumn<int>(
                name: "Round",
                table: "Interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Interviews",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailFailureReason",
                table: "Interviews",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmailNotificationStatus",
                table: "Interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailSentAt",
                table: "Interviews",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InterviewRoomId",
                table: "Interviews",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterviewType",
                table: "Interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "InterviewVenueId",
                table: "Interviews",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledEndAt",
                table: "Interviews",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledStartAt",
                table: "Interviews",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SmsNotificationStatus",
                table: "Interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InterviewPanelMembers",
                columns: table => new
                {
                    InterviewId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewPanelMembers", x => new { x.InterviewId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_InterviewPanelMembers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewPanelMembers_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "InterviewId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewVenues",
                columns: table => new
                {
                    InterviewVenueId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VenueName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
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
                    table.PrimaryKey("PK_InterviewVenues", x => x.InterviewVenueId);
                });

            migrationBuilder.CreateTable(
                name: "InterviewRooms",
                columns: table => new
                {
                    InterviewRoomId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InterviewVenueId = table.Column<long>(type: "bigint", nullable: false),
                    RoomName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_InterviewRooms", x => x.InterviewRoomId);
                    table.ForeignKey(
                        name: "FK_InterviewRooms_InterviewVenues_InterviewVenueId",
                        column: x => x.InterviewVenueId,
                        principalTable: "InterviewVenues",
                        principalColumn: "InterviewVenueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_InterviewRoomId",
                table: "Interviews",
                column: "InterviewRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_InterviewVenueId",
                table: "Interviews",
                column: "InterviewVenueId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewPanelMembers_EmployeeId",
                table: "InterviewPanelMembers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRooms_InterviewVenueId",
                table: "InterviewRooms",
                column: "InterviewVenueId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRooms_RoomName",
                table: "InterviewRooms",
                column: "RoomName");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewVenues_VenueName",
                table: "InterviewVenues",
                column: "VenueName");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_InterviewRooms_InterviewRoomId",
                table: "Interviews",
                column: "InterviewRoomId",
                principalTable: "InterviewRooms",
                principalColumn: "InterviewRoomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_InterviewVenues_InterviewVenueId",
                table: "Interviews",
                column: "InterviewVenueId",
                principalTable: "InterviewVenues",
                principalColumn: "InterviewVenueId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_InterviewRooms_InterviewRoomId",
                table: "Interviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_InterviewVenues_InterviewVenueId",
                table: "Interviews");

            migrationBuilder.DropTable(
                name: "InterviewPanelMembers");

            migrationBuilder.DropTable(
                name: "InterviewRooms");

            migrationBuilder.DropTable(
                name: "InterviewVenues");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_InterviewRoomId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_InterviewVenueId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "EmailFailureReason",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "EmailNotificationStatus",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "EmailSentAt",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "InterviewRoomId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "InterviewType",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "InterviewVenueId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "ScheduledEndAt",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "ScheduledStartAt",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "SmsNotificationStatus",
                table: "Interviews");

            migrationBuilder.RenameColumn(
                name: "SmsLoggedAt",
                table: "Interviews",
                newName: "ScheduledDate");

            migrationBuilder.RenameColumn(
                name: "PipelineStageId",
                table: "Interviews",
                newName: "InterviewerId");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Interviews",
                newName: "Feedback");

            migrationBuilder.AlterColumn<string>(
                name: "Round",
                table: "Interviews",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Interviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Interviews",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "Interviews",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
