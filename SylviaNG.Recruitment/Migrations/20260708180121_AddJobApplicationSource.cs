using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddJobApplicationSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "JobApplications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "External");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "JobApplications");
        }
    }
}
