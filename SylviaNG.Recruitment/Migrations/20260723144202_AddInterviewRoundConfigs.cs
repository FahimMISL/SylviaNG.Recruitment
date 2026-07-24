using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddInterviewRoundConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InterviewRoundConfigId",
                table: "Interviews",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Result",
                table: "Interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InterviewRoundConfigs",
                columns: table => new
                {
                    InterviewRoundConfigId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobPostingId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    ScorecardId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewRoundConfigs", x => x.InterviewRoundConfigId);
                    table.ForeignKey(
                        name: "FK_InterviewRoundConfigs_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "JobPostingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewRoundConfigs_Scorecards_ScorecardId",
                        column: x => x.ScorecardId,
                        principalTable: "Scorecards",
                        principalColumn: "ScorecardId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewRoundConfigPanelMembers",
                columns: table => new
                {
                    InterviewRoundConfigId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewRoundConfigPanelMembers", x => new { x.InterviewRoundConfigId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_InterviewRoundConfigPanelMembers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewRoundConfigPanelMembers_InterviewRoundConfigs_Inte~",
                        column: x => x.InterviewRoundConfigId,
                        principalTable: "InterviewRoundConfigs",
                        principalColumn: "InterviewRoundConfigId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_InterviewRoundConfigId",
                table: "Interviews",
                column: "InterviewRoundConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRoundConfigPanelMembers_EmployeeId",
                table: "InterviewRoundConfigPanelMembers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRoundConfigs_JobPostingId_Sequence",
                table: "InterviewRoundConfigs",
                columns: new[] { "JobPostingId", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRoundConfigs_ScorecardId",
                table: "InterviewRoundConfigs",
                column: "ScorecardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_InterviewRoundConfigs_InterviewRoundConfigId",
                table: "Interviews",
                column: "InterviewRoundConfigId",
                principalTable: "InterviewRoundConfigs",
                principalColumn: "InterviewRoundConfigId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_InterviewRoundConfigs_InterviewRoundConfigId",
                table: "Interviews");

            migrationBuilder.DropTable(
                name: "InterviewRoundConfigPanelMembers");

            migrationBuilder.DropTable(
                name: "InterviewRoundConfigs");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_InterviewRoundConfigId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "InterviewRoundConfigId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "Result",
                table: "Interviews");
        }
    }
}
