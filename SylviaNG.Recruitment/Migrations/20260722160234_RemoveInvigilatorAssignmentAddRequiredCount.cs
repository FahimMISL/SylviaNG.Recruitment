using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInvigilatorAssignmentAddRequiredCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamRoomInvigilators");

            migrationBuilder.DropColumn(
                name: "NotifyInvigilatorsOnAssign",
                table: "ExamRooms");

            migrationBuilder.AddColumn<int>(
                name: "RequiredInvigilatorCount",
                table: "ExamRooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredInvigilatorCount",
                table: "ExamRooms");

            migrationBuilder.AddColumn<bool>(
                name: "NotifyInvigilatorsOnAssign",
                table: "ExamRooms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ExamRoomInvigilators",
                columns: table => new
                {
                    ExamRoomId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamRoomInvigilators", x => new { x.ExamRoomId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_ExamRoomInvigilators_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamRoomInvigilators_ExamRooms_ExamRoomId",
                        column: x => x.ExamRoomId,
                        principalTable: "ExamRooms",
                        principalColumn: "ExamRoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamRoomInvigilators_EmployeeId",
                table: "ExamRoomInvigilators",
                column: "EmployeeId");
        }
    }
}
