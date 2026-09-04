using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedPipelineStageFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PipelineStageInterviewers");

            migrationBuilder.DropColumn(
                name: "AllowCandidateReschedule",
                table: "PipelineStages");

            migrationBuilder.DropColumn(
                name: "ColorBadge",
                table: "PipelineStages");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "PipelineStages");

            migrationBuilder.DropColumn(
                name: "EmailTemplate",
                table: "PipelineStages");

            migrationBuilder.DropColumn(
                name: "NotifyCandidateOnEnter",
                table: "PipelineStages");

            migrationBuilder.DropColumn(
                name: "NotifyInterviewersOnAssign",
                table: "PipelineStages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowCandidateReschedule",
                table: "PipelineStages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ColorBadge",
                table: "PipelineStages",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "PipelineStages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailTemplate",
                table: "PipelineStages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyCandidateOnEnter",
                table: "PipelineStages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyInterviewersOnAssign",
                table: "PipelineStages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PipelineStageInterviewers",
                columns: table => new
                {
                    PipelineStageId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineStageInterviewers", x => new { x.PipelineStageId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_PipelineStageInterviewers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PipelineStageInterviewers_PipelineStages_PipelineStageId",
                        column: x => x.PipelineStageId,
                        principalTable: "PipelineStages",
                        principalColumn: "PipelineStageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStageInterviewers_EmployeeId",
                table: "PipelineStageInterviewers",
                column: "EmployeeId");
        }
    }
}
