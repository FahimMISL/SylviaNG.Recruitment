using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddUniversityLibraryAndGradingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GradingSystem",
                table: "CandidateEducations",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UniversityLibraryItemId",
                table: "CandidateEducations",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UniversityLibraryItems",
                columns: table => new
                {
                    UniversityLibraryItemId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
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
                    table.PrimaryKey("PK_UniversityLibraryItems", x => x.UniversityLibraryItemId);
                });

            migrationBuilder.InsertData(
                table: "UniversityLibraryItems",
                columns: new[] { "UniversityLibraryItemId", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, "DU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "University of Dhaka", null, 1, "default_tenant", null, null },
                    { 2L, "RU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "University of Rajshahi", null, 1, "default_tenant", null, null },
                    { 3L, "CU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "University of Chittagong", null, 1, "default_tenant", null, null },
                    { 4L, "JU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Jahangirnagar University", null, 1, "default_tenant", null, null },
                    { 5L, "BUET", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bangladesh University of Engineering and Technology", null, 1, "default_tenant", null, null },
                    { 6L, "KU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Khulna University", null, 1, "default_tenant", null, null },
                    { 7L, "SUST", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Shahjalal University of Science and Technology", null, 1, "default_tenant", null, null },
                    { 8L, "IU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Islamic University, Bangladesh", null, 1, "default_tenant", null, null },
                    { 9L, "JnU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Jagannath University", null, 1, "default_tenant", null, null },
                    { 10L, "CoU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Comilla University", null, 1, "default_tenant", null, null },
                    { 11L, "JKKNIU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Jatiya Kabi Kazi Nazrul Islam University", null, 1, "default_tenant", null, null },
                    { 12L, "BRUR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Begum Rokeya University, Rangpur", null, 1, "default_tenant", null, null },
                    { 13L, "NSTU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Noakhali Science and Technology University", null, 1, "default_tenant", null, null },
                    { 14L, "PSTU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Patuakhali Science and Technology University", null, 1, "default_tenant", null, null },
                    { 15L, "CUET", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Chittagong University of Engineering & Technology", null, 1, "default_tenant", null, null },
                    { 16L, "RUET", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Rajshahi University of Engineering & Technology", null, 1, "default_tenant", null, null },
                    { 17L, "KUET", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Khulna University of Engineering & Technology", null, 1, "default_tenant", null, null },
                    { 18L, "DUET", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Dhaka University of Engineering & Technology, Gazipur", null, 1, "default_tenant", null, null },
                    { 19L, "BAU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bangladesh Agricultural University", null, 1, "default_tenant", null, null },
                    { 20L, "SAU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Sher-e-Bangla Agricultural University", null, 1, "default_tenant", null, null },
                    { 21L, "SYLAU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Sylhet Agricultural University", null, 1, "default_tenant", null, null },
                    { 22L, "BSMRAU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bangabandhu Sheikh Mujibur Rahman Agricultural University", null, 1, "default_tenant", null, null },
                    { 23L, "HSTU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Hajee Mohammad Danesh Science and Technology University", null, 1, "default_tenant", null, null },
                    { 24L, "PUST", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Pabna University of Science and Technology", null, 1, "default_tenant", null, null },
                    { 25L, "JUST", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Jessore University of Science and Technology", null, 1, "default_tenant", null, null },
                    { 26L, "BSMMU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bangabandhu Sheikh Mujib Medical University", null, 1, "default_tenant", null, null },
                    { 27L, "NU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "National University, Bangladesh", null, 1, "default_tenant", null, null },
                    { 28L, "BOU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bangladesh Open University", null, 1, "default_tenant", null, null },
                    { 29L, "BUP", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bangladesh University of Professionals", null, 1, "default_tenant", null, null },
                    { 30L, "MBSTU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Mawlana Bhashani Science and Technology University", null, 1, "default_tenant", null, null },
                    { 31L, "BUTEX", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bangladesh University of Textiles", null, 1, "default_tenant", null, null },
                    { 32L, "NSU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "North South University", null, 1, "default_tenant", null, null },
                    { 33L, "BRACU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "BRAC University", null, 1, "default_tenant", null, null },
                    { 34L, "IUB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Independent University, Bangladesh", null, 1, "default_tenant", null, null },
                    { 35L, "AIUB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "American International University-Bangladesh", null, 1, "default_tenant", null, null },
                    { 36L, "AUST", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Ahsanullah University of Science and Technology", null, 1, "default_tenant", null, null },
                    { 37L, "IUT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Islamic University of Technology", null, 1, "default_tenant", null, null },
                    { 38L, "UIU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "United International University", null, 1, "default_tenant", null, null },
                    { 39L, "EWU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "East West University", null, 1, "default_tenant", null, null },
                    { 40L, "ULAB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "University of Liberal Arts Bangladesh", null, 1, "default_tenant", null, null },
                    { 41L, "DIU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Daffodil International University", null, 1, "default_tenant", null, null },
                    { 42L, "SEU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Southeast University", null, 1, "default_tenant", null, null },
                    { 43L, "GUB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Green University of Bangladesh", null, 1, "default_tenant", null, null },
                    { 44L, "SUB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Stamford University Bangladesh", null, 1, "default_tenant", null, null },
                    { 45L, "WUB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "World University of Bangladesh", null, 1, "default_tenant", null, null },
                    { 46L, "PU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Presidency University", null, 1, "default_tenant", null, null },
                    { 47L, "IIUC", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "International Islamic University Chittagong", null, 1, "default_tenant", null, null },
                    { 48L, "IUBAT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "International University of Business Agriculture and Technology", null, 1, "default_tenant", null, null },
                    { 49L, "PU2", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Prime University", null, 1, "default_tenant", null, null },
                    { 50L, "CityU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "City University", null, 1, "default_tenant", null, null },
                    { 51L, "NUB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Northern University Bangladesh", null, 1, "default_tenant", null, null },
                    { 52L, "MIU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Manarat International University", null, 1, "default_tenant", null, null },
                    { 53L, "UU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Uttara University", null, 1, "default_tenant", null, null },
                    { 54L, "UAP", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "University of Asia Pacific", null, 1, "default_tenant", null, null },
                    { 55L, "AUB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Asian University of Bangladesh", null, 1, "default_tenant", null, null },
                    { 56L, "EU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Eastern University", null, 1, "default_tenant", null, null },
                    { 57L, "PUC", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Premier University, Chattogram", null, 1, "default_tenant", null, null },
                    { 58L, "LUS", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Leading University, Sylhet", null, 1, "default_tenant", null, null },
                    { 59L, "MU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Metropolitan University, Sylhet", null, 1, "default_tenant", null, null },
                    { 60L, "SMUCT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Shanto-Mariam University of Creative Technology", null, 1, "default_tenant", null, null },
                    { 61L, "ASAUB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "ASA University Bangladesh", null, 1, "default_tenant", null, null },
                    { 62L, "VUB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Victoria University of Bangladesh", null, 1, "default_tenant", null, null },
                    { 63L, "SUBD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Southern University Bangladesh", null, 1, "default_tenant", null, null },
                    { 64L, "UODA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "University of Development Alternative", null, 1, "default_tenant", null, null },
                    { 65L, "CWU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Central Women's University", null, 1, "default_tenant", null, null },
                    { 66L, "FU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Feni University", null, 1, "default_tenant", null, null },
                    { 67L, "BRIT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Britannia University", null, 1, "default_tenant", null, null },
                    { 68L, "NDUB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Notre Dame University Bangladesh", null, 1, "default_tenant", null, null },
                    { 69L, "VU2", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Varendra University", null, 1, "default_tenant", null, null },
                    { 70L, "BAUST", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bangladesh Army University of Science and Technology", null, 1, "default_tenant", null, null },
                    { 71L, "MIST", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Military Institute of Science and Technology", null, 1, "default_tenant", null, null },
                    { 72L, "BUBT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bangladesh University of Business and Technology", null, 1, "default_tenant", null, null },
                    { 73L, "CUB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Canadian University of Bangladesh", null, 1, "default_tenant", null, null },
                    { 74L, "EUB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "European University of Bangladesh", null, 1, "default_tenant", null, null },
                    { 75L, "GB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Gono Bishwabidyalay", null, 1, "default_tenant", null, null },
                    { 76L, "NWU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "North Western University", null, 1, "default_tenant", null, null },
                    { 77L, "SU2", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Sonargaon University", null, 1, "default_tenant", null, null },
                    { 78L, "UITS", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "University of Information Technology and Sciences", null, 1, "default_tenant", null, null },
                    { 79L, "ZHSUST", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Z. H. Sikder University of Science & Technology", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEducations_UniversityLibraryItemId",
                table: "CandidateEducations",
                column: "UniversityLibraryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UniversityLibraryItems_Name",
                table: "UniversityLibraryItems",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateEducations_UniversityLibraryItems_UniversityLibrar~",
                table: "CandidateEducations",
                column: "UniversityLibraryItemId",
                principalTable: "UniversityLibraryItems",
                principalColumn: "UniversityLibraryItemId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateEducations_UniversityLibraryItems_UniversityLibrar~",
                table: "CandidateEducations");

            migrationBuilder.DropTable(
                name: "UniversityLibraryItems");

            migrationBuilder.DropIndex(
                name: "IX_CandidateEducations_UniversityLibraryItemId",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "GradingSystem",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "UniversityLibraryItemId",
                table: "CandidateEducations");
        }
    }
}
