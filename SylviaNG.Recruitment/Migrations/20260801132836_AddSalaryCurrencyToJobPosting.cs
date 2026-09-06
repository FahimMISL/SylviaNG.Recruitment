using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddSalaryCurrencyToJobPosting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalaryCurrency",
                table: "JobPostings",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 1L,
                column: "SalaryCurrency",
                value: null);

            migrationBuilder.UpdateData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 2L,
                column: "SalaryCurrency",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryCurrency",
                table: "JobPostings");
        }
    }
}
