using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddImpersonationAndProfileFieldConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImpersonationSessions",
                columns: table => new
                {
                    ImpersonationSessionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActorUserAccountId = table.Column<long>(type: "bigint", nullable: false),
                    TargetUserAccountId = table.Column<long>(type: "bigint", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpersonationSessions", x => x.ImpersonationSessionId);
                    table.ForeignKey(
                        name: "FK_ImpersonationSessions_UserAccounts_ActorUserAccountId",
                        column: x => x.ActorUserAccountId,
                        principalTable: "UserAccounts",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImpersonationSessions_UserAccounts_TargetUserAccountId",
                        column: x => x.TargetUserAccountId,
                        principalTable: "UserAccounts",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfileFieldConfigs",
                columns: table => new
                {
                    ProfileFieldConfigId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Field = table.Column<int>(type: "integer", nullable: false),
                    JobPostingId = table.Column<long>(type: "bigint", nullable: true),
                    Visibility = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileFieldConfigs", x => x.ProfileFieldConfigId);
                    table.ForeignKey(
                        name: "FK_ProfileFieldConfigs_JobPostings_JobPostingId",
                        column: x => x.JobPostingId,
                        principalTable: "JobPostings",
                        principalColumn: "JobPostingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImpersonationLogs",
                columns: table => new
                {
                    ImpersonationLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImpersonationSessionId = table.Column<long>(type: "bigint", nullable: false),
                    HttpMethod = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpersonationLogs", x => x.ImpersonationLogId);
                    table.ForeignKey(
                        name: "FK_ImpersonationLogs_ImpersonationSessions_ImpersonationSessio~",
                        column: x => x.ImpersonationSessionId,
                        principalTable: "ImpersonationSessions",
                        principalColumn: "ImpersonationSessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImpersonationLogs_ImpersonationSessionId",
                table: "ImpersonationLogs",
                column: "ImpersonationSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ImpersonationSessions_ActorUserAccountId",
                table: "ImpersonationSessions",
                column: "ActorUserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImpersonationSessions_TargetUserAccountId",
                table: "ImpersonationSessions",
                column: "TargetUserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileFieldConfigs_JobPostingId",
                table: "ProfileFieldConfigs",
                column: "JobPostingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImpersonationLogs");

            migrationBuilder.DropTable(
                name: "ProfileFieldConfigs");

            migrationBuilder.DropTable(
                name: "ImpersonationSessions");
        }
    }
}
