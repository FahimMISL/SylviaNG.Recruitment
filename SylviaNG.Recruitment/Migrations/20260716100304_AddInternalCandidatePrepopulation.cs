using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddInternalCandidatePrepopulation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employees",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DesignationId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrepopulatedFullName",
                table: "CandidateProfiles",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrepopulatedPhone",
                table: "CandidateProfiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "DesignationId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "PrepopulatedFullName",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "PrepopulatedPhone",
                table: "CandidateProfiles");
        }
    }
}
