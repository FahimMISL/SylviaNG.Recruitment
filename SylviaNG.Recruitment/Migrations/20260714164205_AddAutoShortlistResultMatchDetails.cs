using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoShortlistResultMatchDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExperienceBand",
                table: "AutoShortlistResults",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatchedSkills",
                table: "AutoShortlistResults",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperienceBand",
                table: "AutoShortlistResults");

            migrationBuilder.DropColumn(
                name: "MatchedSkills",
                table: "AutoShortlistResults");
        }
    }
}
