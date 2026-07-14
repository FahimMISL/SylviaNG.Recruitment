using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddHiringPipeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "HiringPipelineId",
                table: "JobPostings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HiringPipelines",
                columns: table => new
                {
                    HiringPipelineId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_HiringPipelines", x => x.HiringPipelineId);
                });

            migrationBuilder.CreateTable(
                name: "PipelineStages",
                columns: table => new
                {
                    PipelineStageId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HiringPipelineId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StageType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PassingCriteria = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: true),
                    EstimatedDurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    SlaDays = table.Column<int>(type: "integer", nullable: true),
                    ColorBadge = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmailTemplate = table.Column<string>(type: "text", nullable: true),
                    NotifyCandidateOnEnter = table.Column<bool>(type: "boolean", nullable: false),
                    NotifyInterviewersOnAssign = table.Column<bool>(type: "boolean", nullable: false),
                    RequiredDocuments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AllowCandidateReschedule = table.Column<bool>(type: "boolean", nullable: false),
                    AutoProgressionRule = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ManualApprovalRequired = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_PipelineStages", x => x.PipelineStageId);
                    table.ForeignKey(
                        name: "FK_PipelineStages_HiringPipelines_HiringPipelineId",
                        column: x => x.HiringPipelineId,
                        principalTable: "HiringPipelines",
                        principalColumn: "HiringPipelineId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.UpdateData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 1L,
                column: "HiringPipelineId",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 2L,
                column: "HiringPipelineId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_HiringPipelineId",
                table: "JobPostings",
                column: "HiringPipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_HiringPipelines_Name",
                table: "HiringPipelines",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStageInterviewers_EmployeeId",
                table: "PipelineStageInterviewers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStages_HiringPipelineId_DisplayOrder",
                table: "PipelineStages",
                columns: new[] { "HiringPipelineId", "DisplayOrder" });

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_HiringPipelines_HiringPipelineId",
                table: "JobPostings",
                column: "HiringPipelineId",
                principalTable: "HiringPipelines",
                principalColumn: "HiringPipelineId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_HiringPipelines_HiringPipelineId",
                table: "JobPostings");

            migrationBuilder.DropTable(
                name: "PipelineStageInterviewers");

            migrationBuilder.DropTable(
                name: "PipelineStages");

            migrationBuilder.DropTable(
                name: "HiringPipelines");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_HiringPipelineId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "HiringPipelineId",
                table: "JobPostings");
        }
    }
}
