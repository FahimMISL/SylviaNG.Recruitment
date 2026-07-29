using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddExamAndExamEnrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    ExamId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobPostingId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ScheduledStartAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    TotalMarks = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    PassMarks = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    ExamType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ExamVenueId = table.Column<long>(type: "bigint", nullable: true),
                    QuestionGroupId = table.Column<long>(type: "bigint", nullable: true),
                    SeatPlanGeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_Exams", x => x.ExamId);
                    table.ForeignKey(
                        name: "FK_Exams_ExamVenues_ExamVenueId",
                        column: x => x.ExamVenueId,
                        principalTable: "ExamVenues",
                        principalColumn: "ExamVenueId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exams_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "JobPostingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exams_QuestionGroups_QuestionGroupId",
                        column: x => x.QuestionGroupId,
                        principalTable: "QuestionGroups",
                        principalColumn: "QuestionGroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamEnrollments",
                columns: table => new
                {
                    ExamEnrollmentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExamId = table.Column<long>(type: "bigint", nullable: false),
                    JobApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    ExamRoomId = table.Column<long>(type: "bigint", nullable: true),
                    SeatNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EnrolledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmailNotificationStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmailSentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmailFailureReason = table.Column<string>(type: "text", nullable: true),
                    SmsNotificationStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SmsLoggedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_ExamEnrollments", x => x.ExamEnrollmentId);
                    table.ForeignKey(
                        name: "FK_ExamEnrollments_ExamRooms_ExamRoomId",
                        column: x => x.ExamRoomId,
                        principalTable: "ExamRooms",
                        principalColumn: "ExamRoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamEnrollments_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "ExamId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamEnrollments_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "JobApplicationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamEnrollments_ExamId_ExamRoomId",
                table: "ExamEnrollments",
                columns: new[] { "ExamId", "ExamRoomId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamEnrollments_ExamId_JobApplicationId",
                table: "ExamEnrollments",
                columns: new[] { "ExamId", "JobApplicationId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamEnrollments_ExamRoomId",
                table: "ExamEnrollments",
                column: "ExamRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamEnrollments_JobApplicationId",
                table: "ExamEnrollments",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_ExamVenueId",
                table: "Exams",
                column: "ExamVenueId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_JobPostingId",
                table: "Exams",
                column: "JobPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_QuestionGroupId",
                table: "Exams",
                column: "QuestionGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamEnrollments");

            migrationBuilder.DropTable(
                name: "Exams");
        }
    }
}
