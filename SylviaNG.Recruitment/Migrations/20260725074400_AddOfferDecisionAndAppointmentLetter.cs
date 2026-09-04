using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddOfferDecisionAndAppointmentLetter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionAt",
                table: "OfferLetters",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeclineReason",
                table: "OfferLetters",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppointmentLetters",
                columns: table => new
                {
                    AppointmentLetterId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    OfferLetterId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    FinalBody = table.Column<string>(type: "text", nullable: false),
                    GeneratedPdfPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_AppointmentLetters", x => x.AppointmentLetterId);
                    table.ForeignKey(
                        name: "FK_AppointmentLetters_DocumentTemplates_DocumentTemplateId",
                        column: x => x.DocumentTemplateId,
                        principalTable: "DocumentTemplates",
                        principalColumn: "DocumentTemplateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentLetters_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "JobApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentLetters_OfferLetters_OfferLetterId",
                        column: x => x.OfferLetterId,
                        principalTable: "OfferLetters",
                        principalColumn: "OfferLetterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentLetters_DocumentTemplateId",
                table: "AppointmentLetters",
                column: "DocumentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentLetters_JobApplicationId",
                table: "AppointmentLetters",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentLetters_OfferLetterId",
                table: "AppointmentLetters",
                column: "OfferLetterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentLetters");

            migrationBuilder.DropColumn(
                name: "DecisionAt",
                table: "OfferLetters");

            migrationBuilder.DropColumn(
                name: "DeclineReason",
                table: "OfferLetters");
        }
    }
}
