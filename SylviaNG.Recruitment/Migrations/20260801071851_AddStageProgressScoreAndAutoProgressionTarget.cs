using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddStageProgressScoreAndAutoProgressionTarget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AutoProgressionTargetDisplayOrder",
                table: "PipelineStages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Score",
                table: "JobApplicationStageProgresses",
                type: "numeric(6,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoProgressionTargetDisplayOrder",
                table: "PipelineStages");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "JobApplicationStageProgresses");
        }
    }
}
