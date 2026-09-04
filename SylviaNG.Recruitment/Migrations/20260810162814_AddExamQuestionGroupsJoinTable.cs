using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddExamQuestionGroupsJoinTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExamQuestionGroups",
                columns: table => new
                {
                    ExamId = table.Column<long>(type: "bigint", nullable: false),
                    QuestionGroupId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQuestionGroups", x => new { x.ExamId, x.QuestionGroupId });
                    table.ForeignKey(
                        name: "FK_ExamQuestionGroups_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "ExamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamQuestionGroups_QuestionGroups_QuestionGroupId",
                        column: x => x.QuestionGroupId,
                        principalTable: "QuestionGroups",
                        principalColumn: "QuestionGroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestionGroups_QuestionGroupId",
                table: "ExamQuestionGroups",
                column: "QuestionGroupId");

            // Backfill: every existing Exam.QuestionGroupId becomes its one row in the new join
            // table, before the column itself is dropped below - preserves existing exams' single
            // group as the equivalent of "still selected", just now through the multi-select path.
            migrationBuilder.Sql(
                """
                INSERT INTO "ExamQuestionGroups" ("ExamId", "QuestionGroupId")
                SELECT "ExamId", "QuestionGroupId" FROM "Exams" WHERE "QuestionGroupId" IS NOT NULL;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_QuestionGroups_QuestionGroupId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_QuestionGroupId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "QuestionGroupId",
                table: "Exams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "QuestionGroupId",
                table: "Exams",
                type: "bigint",
                nullable: true);

            // Best-effort backfill: an exam that ended up with more than one group in the
            // multi-select can only keep one going back to the single-FK shape - picks the
            // lowest QuestionGroupId deterministically rather than an arbitrary one.
            migrationBuilder.Sql(
                """
                UPDATE "Exams" e
                SET "QuestionGroupId" = sub."QuestionGroupId"
                FROM (
                    SELECT DISTINCT ON ("ExamId") "ExamId", "QuestionGroupId"
                    FROM "ExamQuestionGroups"
                    ORDER BY "ExamId", "QuestionGroupId"
                ) sub
                WHERE e."ExamId" = sub."ExamId";
                """);

            migrationBuilder.DropTable(
                name: "ExamQuestionGroups");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_QuestionGroupId",
                table: "Exams",
                column: "QuestionGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_QuestionGroups_QuestionGroupId",
                table: "Exams",
                column: "QuestionGroupId",
                principalTable: "QuestionGroups",
                principalColumn: "QuestionGroupId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
