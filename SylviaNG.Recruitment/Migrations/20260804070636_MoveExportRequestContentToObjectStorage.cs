using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class MoveExportRequestContentToObjectStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "ExportRequests");

            migrationBuilder.AddColumn<string>(
                name: "ContentObjectKey",
                table: "ExportRequests",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentObjectKey",
                table: "ExportRequests");

            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "ExportRequests",
                type: "bytea",
                nullable: true);
        }
    }
}
