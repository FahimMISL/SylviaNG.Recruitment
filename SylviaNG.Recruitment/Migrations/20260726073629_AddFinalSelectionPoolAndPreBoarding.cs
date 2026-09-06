using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalSelectionPoolAndPreBoarding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinalSelectionPools",
                columns: table => new
                {
                    FinalSelectionPoolId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfferLetterId = table.Column<long>(type: "bigint", nullable: false),
                    JobApplicationId = table.Column<long>(type: "bigint", nullable: false),
                    BatchLabel = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    JoiningDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HasJoined = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EnteredPoolAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_FinalSelectionPools", x => x.FinalSelectionPoolId);
                    table.ForeignKey(
                        name: "FK_FinalSelectionPools_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "JobApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinalSelectionPools_OfferLetters_OfferLetterId",
                        column: x => x.OfferLetterId,
                        principalTable: "OfferLetters",
                        principalColumn: "OfferLetterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreBoardingSubmissions",
                columns: table => new
                {
                    PreBoardingSubmissionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FinalSelectionPoolId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmergencyContactName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EmergencyContactRelationship = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EmergencyContactPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InsuranceProvider = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    InsurancePolicyNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InsuranceNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    BankName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BankBranch = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BankAccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BankAccountNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BankRoutingNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreBoardingSubmissions", x => x.PreBoardingSubmissionId);
                    table.ForeignKey(
                        name: "FK_PreBoardingSubmissions_FinalSelectionPools_FinalSelectionPo~",
                        column: x => x.FinalSelectionPoolId,
                        principalTable: "FinalSelectionPools",
                        principalColumn: "FinalSelectionPoolId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreBoardingNominees",
                columns: table => new
                {
                    PreBoardingNomineeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreBoardingSubmissionId = table.Column<long>(type: "bigint", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Relationship = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SharePercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ContactPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_PreBoardingNominees", x => x.PreBoardingNomineeId);
                    table.ForeignKey(
                        name: "FK_PreBoardingNominees_PreBoardingSubmissions_PreBoardingSubmi~",
                        column: x => x.PreBoardingSubmissionId,
                        principalTable: "PreBoardingSubmissions",
                        principalColumn: "PreBoardingSubmissionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinalSelectionPools_JobApplicationId",
                table: "FinalSelectionPools",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_FinalSelectionPools_OfferLetterId",
                table: "FinalSelectionPools",
                column: "OfferLetterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreBoardingNominees_PreBoardingSubmissionId",
                table: "PreBoardingNominees",
                column: "PreBoardingSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_PreBoardingSubmissions_FinalSelectionPoolId",
                table: "PreBoardingSubmissions",
                column: "FinalSelectionPoolId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreBoardingNominees");

            migrationBuilder.DropTable(
                name: "PreBoardingSubmissions");

            migrationBuilder.DropTable(
                name: "FinalSelectionPools");
        }
    }
}
