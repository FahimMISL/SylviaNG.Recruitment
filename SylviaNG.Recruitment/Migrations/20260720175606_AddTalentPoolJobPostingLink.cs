using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddTalentPoolJobPostingLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "JobPostingId",
                table: "TalentPools",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TalentPools_JobPostingId",
                table: "TalentPools",
                column: "JobPostingId");

            migrationBuilder.AddForeignKey(
                name: "FK_TalentPools_JobPostings_JobPostingId",
                table: "TalentPools",
                column: "JobPostingId",
                principalTable: "JobPostings",
                principalColumn: "JobPostingId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TalentPools_JobPostings_JobPostingId",
                table: "TalentPools");

            migrationBuilder.DropIndex(
                name: "IX_TalentPools_JobPostingId",
                table: "TalentPools");

            migrationBuilder.DropColumn(
                name: "JobPostingId",
                table: "TalentPools");
        }
    }
}
