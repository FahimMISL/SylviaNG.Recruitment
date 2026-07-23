using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateProfileIdToJobApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CandidateProfileId",
                table: "JobApplications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_CandidateProfileId",
                table: "JobApplications",
                column: "CandidateProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_CandidateProfiles_CandidateProfileId",
                table: "JobApplications",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "CandidateProfileId",
                onDelete: ReferentialAction.Restrict);

            // Backfill: link existing JobApplication rows to a CandidateProfile wherever exactly
            // one profile matches on email (case-insensitive) - one-time data fix, this is the
            // last place the codebase ever needs to reason about email-vs-profile matching for
            // pre-existing rows going forward.
            migrationBuilder.Sql(@"
                UPDATE ""JobApplications"" ja
                SET ""CandidateProfileId"" = matched.""CandidateProfileId""
                FROM (
                    SELECT LOWER(""Email"") AS email_lower, MIN(""CandidateProfileId"") AS ""CandidateProfileId""
                    FROM ""CandidateProfiles""
                    GROUP BY LOWER(""Email"")
                    HAVING COUNT(*) = 1
                ) AS matched
                WHERE ja.""CandidateEmail"" IS NOT NULL
                  AND LOWER(ja.""CandidateEmail"") = matched.email_lower
                  AND ja.""CandidateProfileId"" IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_CandidateProfiles_CandidateProfileId",
                table: "JobApplications");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_CandidateProfileId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "CandidateProfileId",
                table: "JobApplications");
        }
    }
}
