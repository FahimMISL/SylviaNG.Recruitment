using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddWaiverRulesSpecialCategoryReferralSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReferralSourceId",
                table: "JobApplications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SpecialCategoryId",
                table: "JobApplications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WaivedAt",
                table: "JobApplications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WaiverRuleId",
                table: "JobApplications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReferralSources",
                columns: table => new
                {
                    ReferralSourceId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_ReferralSources", x => x.ReferralSourceId);
                });

            migrationBuilder.CreateTable(
                name: "SpecialCategories",
                columns: table => new
                {
                    SpecialCategoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_SpecialCategories", x => x.SpecialCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "WaiverRules",
                columns: table => new
                {
                    WaiverRuleId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CandidateTypeFilter = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SpecialCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    ReferralSourceId = table.Column<long>(type: "bigint", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_WaiverRules", x => x.WaiverRuleId);
                    table.ForeignKey(
                        name: "FK_WaiverRules_ReferralSources_ReferralSourceId",
                        column: x => x.ReferralSourceId,
                        principalTable: "ReferralSources",
                        principalColumn: "ReferralSourceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaiverRules_SpecialCategories_SpecialCategoryId",
                        column: x => x.SpecialCategoryId,
                        principalTable: "SpecialCategories",
                        principalColumn: "SpecialCategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_JobApplicationId_PaymentStatus",
                table: "Payments",
                columns: new[] { "JobApplicationId", "PaymentStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_ReferralSourceId",
                table: "JobApplications",
                column: "ReferralSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_SpecialCategoryId",
                table: "JobApplications",
                column: "SpecialCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_WaiverRuleId",
                table: "JobApplications",
                column: "WaiverRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralSources_Name",
                table: "ReferralSources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecialCategories_Name",
                table: "SpecialCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaiverRules_Name",
                table: "WaiverRules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaiverRules_Priority",
                table: "WaiverRules",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverRules_ReferralSourceId",
                table: "WaiverRules",
                column: "ReferralSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_WaiverRules_SpecialCategoryId",
                table: "WaiverRules",
                column: "SpecialCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_ReferralSources_ReferralSourceId",
                table: "JobApplications",
                column: "ReferralSourceId",
                principalTable: "ReferralSources",
                principalColumn: "ReferralSourceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_SpecialCategories_SpecialCategoryId",
                table: "JobApplications",
                column: "SpecialCategoryId",
                principalTable: "SpecialCategories",
                principalColumn: "SpecialCategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_WaiverRules_WaiverRuleId",
                table: "JobApplications",
                column: "WaiverRuleId",
                principalTable: "WaiverRules",
                principalColumn: "WaiverRuleId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_ReferralSources_ReferralSourceId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_SpecialCategories_SpecialCategoryId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_WaiverRules_WaiverRuleId",
                table: "JobApplications");

            migrationBuilder.DropTable(
                name: "WaiverRules");

            migrationBuilder.DropTable(
                name: "ReferralSources");

            migrationBuilder.DropTable(
                name: "SpecialCategories");

            migrationBuilder.DropIndex(
                name: "IX_Payments_JobApplicationId_PaymentStatus",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_ReferralSourceId",
                table: "JobApplications");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_SpecialCategoryId",
                table: "JobApplications");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_WaiverRuleId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "ReferralSourceId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "SpecialCategoryId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "WaivedAt",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "WaiverRuleId",
                table: "JobApplications");
        }
    }
}
