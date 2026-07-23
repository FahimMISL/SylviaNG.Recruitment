using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class MergeAssessmentWorkflowIntoHiringPipeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_AssessmentWorkflows_AssessmentWorkflowId",
                table: "JobPostings");

            migrationBuilder.DropTable(
                name: "AssessmentStages");

            migrationBuilder.DropTable(
                name: "AssessmentWorkflows");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_AssessmentWorkflowId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "AssessmentWorkflowId",
                table: "JobPostings");

            migrationBuilder.AddColumn<int>(
                name: "MaxMarks",
                table: "PipelineStages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassMarks",
                table: "PipelineStages",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxMarks",
                table: "PipelineStages");

            migrationBuilder.DropColumn(
                name: "PassMarks",
                table: "PipelineStages");

            migrationBuilder.AddColumn<long>(
                name: "AssessmentWorkflowId",
                table: "JobPostings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssessmentWorkflows",
                columns: table => new
                {
                    AssessmentWorkflowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentWorkflows", x => x.AssessmentWorkflowId);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentStages",
                columns: table => new
                {
                    AssessmentStageId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssessmentWorkflowId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: false),
                    MaxMarks = table.Column<int>(type: "integer", nullable: false),
                    PassMarks = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    StageType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentStages", x => x.AssessmentStageId);
                    table.ForeignKey(
                        name: "FK_AssessmentStages_AssessmentWorkflows_AssessmentWorkflowId",
                        column: x => x.AssessmentWorkflowId,
                        principalTable: "AssessmentWorkflows",
                        principalColumn: "AssessmentWorkflowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 1L,
                column: "AssessmentWorkflowId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 2L,
                column: "AssessmentWorkflowId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_AssessmentWorkflowId",
                table: "JobPostings",
                column: "AssessmentWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentStages_AssessmentWorkflowId_DisplayOrder",
                table: "AssessmentStages",
                columns: new[] { "AssessmentWorkflowId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentWorkflows_Name",
                table: "AssessmentWorkflows",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_AssessmentWorkflows_AssessmentWorkflowId",
                table: "JobPostings",
                column: "AssessmentWorkflowId",
                principalTable: "AssessmentWorkflows",
                principalColumn: "AssessmentWorkflowId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
