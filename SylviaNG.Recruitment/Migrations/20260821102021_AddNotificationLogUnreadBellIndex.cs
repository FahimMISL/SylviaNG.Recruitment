using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationLogUnreadBellIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_RecipientType_CreatedAt_Unread",
                table: "NotificationLogs",
                columns: new[] { "RecipientType", "CreatedAt" },
                filter: "\"IsRead\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NotificationLogs_RecipientType_CreatedAt_Unread",
                table: "NotificationLogs");
        }
    }
}
