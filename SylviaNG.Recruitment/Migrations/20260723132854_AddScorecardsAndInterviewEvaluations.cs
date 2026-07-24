using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddScorecardsAndInterviewEvaluations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scorecards",
                columns: table => new
                {
                    ScorecardId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_Scorecards", x => x.ScorecardId);
                });

            migrationBuilder.CreateTable(
                name: "InterviewEvaluations",
                columns: table => new
                {
                    InterviewEvaluationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InterviewId = table.Column<long>(type: "bigint", nullable: false),
                    ScorecardId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    OverallComments = table.Column<string>(type: "text", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubmittedByUserName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
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
                    table.PrimaryKey("PK_InterviewEvaluations", x => x.InterviewEvaluationId);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluations_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "InterviewId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluations_Scorecards_ScorecardId",
                        column: x => x.ScorecardId,
                        principalTable: "Scorecards",
                        principalColumn: "ScorecardId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScorecardCriteria",
                columns: table => new
                {
                    ScorecardCriterionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScorecardId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    MaxScore = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_ScorecardCriteria", x => x.ScorecardCriterionId);
                    table.ForeignKey(
                        name: "FK_ScorecardCriteria_Scorecards_ScorecardId",
                        column: x => x.ScorecardId,
                        principalTable: "Scorecards",
                        principalColumn: "ScorecardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewEvaluationScores",
                columns: table => new
                {
                    InterviewEvaluationScoreId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InterviewEvaluationId = table.Column<long>(type: "bigint", nullable: false),
                    ScorecardCriterionId = table.Column<long>(type: "bigint", nullable: false),
                    Score = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
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
                    table.PrimaryKey("PK_InterviewEvaluationScores", x => x.InterviewEvaluationScoreId);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluationScores_InterviewEvaluations_InterviewEva~",
                        column: x => x.InterviewEvaluationId,
                        principalTable: "InterviewEvaluations",
                        principalColumn: "InterviewEvaluationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewEvaluationScores_ScorecardCriteria_ScorecardCriter~",
                        column: x => x.ScorecardCriterionId,
                        principalTable: "ScorecardCriteria",
                        principalColumn: "ScorecardCriterionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluations_InterviewId_EmployeeId",
                table: "InterviewEvaluations",
                columns: new[] { "InterviewId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluations_ScorecardId",
                table: "InterviewEvaluations",
                column: "ScorecardId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationScores_InterviewEvaluationId",
                table: "InterviewEvaluationScores",
                column: "InterviewEvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluationScores_ScorecardCriterionId",
                table: "InterviewEvaluationScores",
                column: "ScorecardCriterionId");

            migrationBuilder.CreateIndex(
                name: "IX_ScorecardCriteria_ScorecardId",
                table: "ScorecardCriteria",
                column: "ScorecardId");

            migrationBuilder.CreateIndex(
                name: "IX_Scorecards_Name",
                table: "Scorecards",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewEvaluationScores");

            migrationBuilder.DropTable(
                name: "InterviewEvaluations");

            migrationBuilder.DropTable(
                name: "ScorecardCriteria");

            migrationBuilder.DropTable(
                name: "Scorecards");
        }
    }
}
