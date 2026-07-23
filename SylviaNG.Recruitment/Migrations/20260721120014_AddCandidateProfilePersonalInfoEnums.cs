using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateProfilePersonalInfoEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "CandidateProfiles",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "CandidateProfiles");
        }
    }
}
