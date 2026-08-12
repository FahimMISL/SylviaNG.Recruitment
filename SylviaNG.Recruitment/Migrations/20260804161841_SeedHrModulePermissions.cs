using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class SeedHrModulePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Action", "Module", "RoleId" },
                values: new object[,]
                {
                    { 0, 0, 2L },
                    { 1, 0, 2L },
                    { 2, 0, 2L },
                    { 3, 0, 2L },
                    { 0, 1, 2L },
                    { 1, 1, 2L },
                    { 2, 1, 2L },
                    { 3, 1, 2L },
                    { 0, 2, 2L },
                    { 1, 2, 2L },
                    { 2, 2, 2L },
                    { 4, 2, 2L },
                    { 0, 3, 2L },
                    { 1, 3, 2L },
                    { 2, 3, 2L },
                    { 4, 3, 2L },
                    { 0, 4, 2L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 0, 0, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 1, 0, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 2, 0, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 3, 0, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 0, 1, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 1, 1, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 2, 1, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 3, 1, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 0, 2, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 1, 2, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 2, 2, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 4, 2, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 0, 3, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 1, 3, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 2, 3, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 4, 3, 2L });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "Action", "Module", "RoleId" },
                keyValues: new object[] { 0, 4, 2L });
        }
    }
}
