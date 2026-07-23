using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddExamTakingAndScoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowResultsToCandidate",
                table: "Exams",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPassed",
                table: "ExamEnrollments",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Score",
                table: "ExamEnrollments",
                type: "numeric(6,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScoreSource",
                table: "ExamEnrollments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScoredAt",
                table: "ExamEnrollments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScoredByUserName",
                table: "ExamEnrollments",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "ExamEnrollments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "ExamEnrollments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExamAnswers",
                columns: table => new
                {
                    ExamAnswerId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExamEnrollmentId = table.Column<long>(type: "bigint", nullable: false),
                    ExamQuestionId = table.Column<long>(type: "bigint", nullable: false),
                    SelectedOptionIds = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AnswerText = table.Column<string>(type: "text", nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: true),
                    MarksAwarded = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
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
                    table.PrimaryKey("PK_ExamAnswers", x => x.ExamAnswerId);
                    table.ForeignKey(
                        name: "FK_ExamAnswers_ExamEnrollments_ExamEnrollmentId",
                        column: x => x.ExamEnrollmentId,
                        principalTable: "ExamEnrollments",
                        principalColumn: "ExamEnrollmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamAnswers_ExamQuestions_ExamQuestionId",
                        column: x => x.ExamQuestionId,
                        principalTable: "ExamQuestions",
                        principalColumn: "ExamQuestionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_ExamEnrollmentId_ExamQuestionId",
                table: "ExamAnswers",
                columns: new[] { "ExamEnrollmentId", "ExamQuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_ExamQuestionId",
                table: "ExamAnswers",
                column: "ExamQuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamAnswers");

            migrationBuilder.DropColumn(
                name: "ShowResultsToCandidate",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "IsPassed",
                table: "ExamEnrollments");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "ExamEnrollments");

            migrationBuilder.DropColumn(
                name: "ScoreSource",
                table: "ExamEnrollments");

            migrationBuilder.DropColumn(
                name: "ScoredAt",
                table: "ExamEnrollments");

            migrationBuilder.DropColumn(
                name: "ScoredByUserName",
                table: "ExamEnrollments");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "ExamEnrollments");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "ExamEnrollments");
        }
    }
}
