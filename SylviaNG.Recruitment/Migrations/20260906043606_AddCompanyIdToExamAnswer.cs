using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyIdToExamAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "ExamAnswers",
                type: "bigint",
                nullable: true);

            // Backfill existing rows from their enrollment's CompanyId - without this, pre-existing
            // ExamAnswer rows would have a null CompanyId and become invisible under the strict
            // ICompanyScoped filter (null != CurrentCompanyId) for any non-SuperAdmin caller.
            migrationBuilder.Sql(@"
                UPDATE ""ExamAnswers"" a
                SET ""CompanyId"" = e.""CompanyId""
                FROM ""ExamEnrollments"" e
                WHERE a.""ExamEnrollmentId"" = e.""ExamEnrollmentId"";
            ");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_CompanyId",
                table: "ExamAnswers",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_Companies_CompanyId",
                table: "JobPostings",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_Companies_CompanyId",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_ExamAnswers_CompanyId",
                table: "ExamAnswers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ExamAnswers");
        }
    }
}
