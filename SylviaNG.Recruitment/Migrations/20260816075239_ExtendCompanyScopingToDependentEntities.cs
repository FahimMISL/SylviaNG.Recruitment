using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class ExtendCompanyScopingToDependentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TalentPools_Name",
                table: "TalentPools");

            migrationBuilder.DropIndex(
                name: "IX_CandidateTalentPools_CandidateProfileId",
                table: "CandidateTalentPools");

            migrationBuilder.AddColumn<string>(
                name: "ExternalEmployeeId",
                table: "UserAccounts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "TargetLetters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "TalentPools",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "OfficeNotes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "OfferLetters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "MedicalLetters",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "JoiningBooklets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "JobPostingAttachments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "JobApplicationStageProgresses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "InterviewRoundConfigs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "InterviewEvaluations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "FitmentDatas",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "FinalSelectionPools",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "ExportRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "Exams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "ExamEnrollments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Companies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "CandidateTalentPools",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "AppointmentLetters",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "CompanyId",
                keyValue: 1L,
                column: "ExternalId",
                value: null);

            // Data fixup: every pre-existing row of these 17 entities (created before this
            // second scoping pass) is backfilled onto the same Default Company the first
            // multi-tenancy migration (AddCompanyMultiTenancy) already backfilled their parent
            // JobPosting/JobApplication/Interview/UserAccount rows onto - keeps them visible
            // under the parent's own already-backfilled CompanyId rather than orphaned with a
            // dangling null.
            migrationBuilder.Sql("UPDATE \"OfferLetters\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"AppointmentLetters\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"MedicalLetters\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"TargetLetters\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"JoiningBooklets\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"OfficeNotes\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"InterviewEvaluations\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"FinalSelectionPools\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"FitmentDatas\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"ExamEnrollments\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"JobApplicationStageProgresses\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"Exams\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"InterviewRoundConfigs\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"JobPostingAttachments\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"TalentPools\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"ExportRequests\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"CandidateTalentPools\" SET \"CompanyId\" = 1 WHERE \"CompanyId\" IS NULL;");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_CompanyId_ExternalEmployeeId",
                table: "UserAccounts",
                columns: new[] { "CompanyId", "ExternalEmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetLetters_CompanyId",
                table: "TargetLetters",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TalentPools_CompanyId_Name",
                table: "TalentPools",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfficeNotes_CompanyId",
                table: "OfficeNotes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLetters_CompanyId",
                table: "OfferLetters",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalLetters_CompanyId",
                table: "MedicalLetters",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JoiningBooklets_CompanyId",
                table: "JoiningBooklets",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostingAttachments_CompanyId",
                table: "JobPostingAttachments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplicationStageProgresses_CompanyId",
                table: "JobApplicationStageProgresses",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRoundConfigs_CompanyId",
                table: "InterviewRoundConfigs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluations_CompanyId",
                table: "InterviewEvaluations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_FitmentDatas_CompanyId",
                table: "FitmentDatas",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_FinalSelectionPools_CompanyId",
                table: "FinalSelectionPools",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportRequests_CompanyId",
                table: "ExportRequests",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CompanyId",
                table: "Exams",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamEnrollments_CompanyId",
                table: "ExamEnrollments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ExternalId",
                table: "Companies",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateTalentPools_CandidateProfileId",
                table: "CandidateTalentPools",
                column: "CandidateProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateTalentPools_CompanyId_CandidateProfileId",
                table: "CandidateTalentPools",
                columns: new[] { "CompanyId", "CandidateProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentLetters_CompanyId",
                table: "AppointmentLetters",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_CompanyId_ExternalEmployeeId",
                table: "UserAccounts");

            migrationBuilder.DropIndex(
                name: "IX_TargetLetters_CompanyId",
                table: "TargetLetters");

            migrationBuilder.DropIndex(
                name: "IX_TalentPools_CompanyId_Name",
                table: "TalentPools");

            migrationBuilder.DropIndex(
                name: "IX_OfficeNotes_CompanyId",
                table: "OfficeNotes");

            migrationBuilder.DropIndex(
                name: "IX_OfferLetters_CompanyId",
                table: "OfferLetters");

            migrationBuilder.DropIndex(
                name: "IX_MedicalLetters_CompanyId",
                table: "MedicalLetters");

            migrationBuilder.DropIndex(
                name: "IX_JoiningBooklets_CompanyId",
                table: "JoiningBooklets");

            migrationBuilder.DropIndex(
                name: "IX_JobPostingAttachments_CompanyId",
                table: "JobPostingAttachments");

            migrationBuilder.DropIndex(
                name: "IX_JobApplicationStageProgresses_CompanyId",
                table: "JobApplicationStageProgresses");

            migrationBuilder.DropIndex(
                name: "IX_InterviewRoundConfigs_CompanyId",
                table: "InterviewRoundConfigs");

            migrationBuilder.DropIndex(
                name: "IX_InterviewEvaluations_CompanyId",
                table: "InterviewEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_FitmentDatas_CompanyId",
                table: "FitmentDatas");

            migrationBuilder.DropIndex(
                name: "IX_FinalSelectionPools_CompanyId",
                table: "FinalSelectionPools");

            migrationBuilder.DropIndex(
                name: "IX_ExportRequests_CompanyId",
                table: "ExportRequests");

            migrationBuilder.DropIndex(
                name: "IX_Exams_CompanyId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_ExamEnrollments_CompanyId",
                table: "ExamEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_Companies_ExternalId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_CandidateTalentPools_CandidateProfileId",
                table: "CandidateTalentPools");

            migrationBuilder.DropIndex(
                name: "IX_CandidateTalentPools_CompanyId_CandidateProfileId",
                table: "CandidateTalentPools");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentLetters_CompanyId",
                table: "AppointmentLetters");

            migrationBuilder.DropColumn(
                name: "ExternalEmployeeId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "TargetLetters");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "TalentPools");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "OfficeNotes");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "OfferLetters");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "MedicalLetters");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "JoiningBooklets");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "JobPostingAttachments");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "JobApplicationStageProgresses");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "InterviewRoundConfigs");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "InterviewEvaluations");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "FitmentDatas");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "FinalSelectionPools");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ExportRequests");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ExamEnrollments");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CandidateTalentPools");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AppointmentLetters");

            migrationBuilder.CreateIndex(
                name: "IX_TalentPools_Name",
                table: "TalentPools",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateTalentPools_CandidateProfileId",
                table: "CandidateTalentPools",
                column: "CandidateProfileId",
                unique: true);
        }
    }
}
