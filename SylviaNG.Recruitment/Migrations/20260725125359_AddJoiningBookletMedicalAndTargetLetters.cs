using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddJoiningBookletMedicalAndTargetLetters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JoiningBooklets",
                columns: table => new
                {
                    JoiningBookletId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    OfferLetterId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    BatchLabel = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    JoiningDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RenderedBody = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_JoiningBooklets", x => x.JoiningBookletId);
                    table.ForeignKey(
                        name: "FK_JoiningBooklets_DocumentTemplates_DocumentTemplateId",
                        column: x => x.DocumentTemplateId,
                        principalTable: "DocumentTemplates",
                        principalColumn: "DocumentTemplateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoiningBooklets_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "JobApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JoiningBooklets_OfferLetters_OfferLetterId",
                        column: x => x.OfferLetterId,
                        principalTable: "OfferLetters",
                        principalColumn: "OfferLetterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicalLetters",
                columns: table => new
                {
                    MedicalLetterId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    OfferLetterId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    MedicalTestCenter = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    RequiredTests = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_MedicalLetters", x => x.MedicalLetterId);
                    table.ForeignKey(
                        name: "FK_MedicalLetters_DocumentTemplates_DocumentTemplateId",
                        column: x => x.DocumentTemplateId,
                        principalTable: "DocumentTemplates",
                        principalColumn: "DocumentTemplateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalLetters_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "JobApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalLetters_OfferLetters_OfferLetterId",
                        column: x => x.OfferLetterId,
                        principalTable: "OfferLetters",
                        principalColumn: "OfferLetterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TargetLetters",
                columns: table => new
                {
                    TargetLetterId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    OfferLetterId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    Kpis = table.Column<string>(type: "text", nullable: false),
                    Objectives = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_TargetLetters", x => x.TargetLetterId);
                    table.ForeignKey(
                        name: "FK_TargetLetters_DocumentTemplates_DocumentTemplateId",
                        column: x => x.DocumentTemplateId,
                        principalTable: "DocumentTemplates",
                        principalColumn: "DocumentTemplateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetLetters_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "JobApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetLetters_OfferLetters_OfferLetterId",
                        column: x => x.OfferLetterId,
                        principalTable: "OfferLetters",
                        principalColumn: "OfferLetterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JoiningBooklets_DocumentTemplateId",
                table: "JoiningBooklets",
                column: "DocumentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_JoiningBooklets_JobApplicationId",
                table: "JoiningBooklets",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_JoiningBooklets_OfferLetterId",
                table: "JoiningBooklets",
                column: "OfferLetterId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalLetters_DocumentTemplateId",
                table: "MedicalLetters",
                column: "DocumentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalLetters_JobApplicationId",
                table: "MedicalLetters",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalLetters_OfferLetterId",
                table: "MedicalLetters",
                column: "OfferLetterId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetLetters_DocumentTemplateId",
                table: "TargetLetters",
                column: "DocumentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetLetters_JobApplicationId",
                table: "TargetLetters",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetLetters_OfferLetterId",
                table: "TargetLetters",
                column: "OfferLetterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoiningBooklets");

            migrationBuilder.DropTable(
                name: "MedicalLetters");

            migrationBuilder.DropTable(
                name: "TargetLetters");
        }
    }
}
