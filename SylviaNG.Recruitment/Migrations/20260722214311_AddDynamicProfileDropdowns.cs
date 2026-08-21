using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicProfileDropdowns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "MobileOperator",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "Religion",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "DegreeTitle",
                table: "CandidateEducations");

            migrationBuilder.AddColumn<long>(
                name: "BloodGroupId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CountryId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GenderId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MaritalStatusId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReligionId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DegreeId",
                table: "CandidateEducations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "EducationBoardId",
                table: "CandidateEducations",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BloodGroups",
                columns: table => new
                {
                    BloodGroupId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_BloodGroups", x => x.BloodGroupId);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    DialCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
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
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Degrees",
                columns: table => new
                {
                    DegreeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_Degrees", x => x.DegreeId);
                });

            migrationBuilder.CreateTable(
                name: "EducationBoards",
                columns: table => new
                {
                    EducationBoardId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
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
                    table.PrimaryKey("PK_EducationBoards", x => x.EducationBoardId);
                });

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    GenderId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_Genders", x => x.GenderId);
                });

            migrationBuilder.CreateTable(
                name: "MaritalStatuses",
                columns: table => new
                {
                    MaritalStatusId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_MaritalStatuses", x => x.MaritalStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Religions",
                columns: table => new
                {
                    ReligionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_Religions", x => x.ReligionId);
                });

            migrationBuilder.InsertData(
                table: "BloodGroups",
                columns: new[] { "BloodGroupId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "A+", null, 1, "default_tenant", null, null },
                    { 2L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "A-", null, 1, "default_tenant", null, null },
                    { 3L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "B+", null, 1, "default_tenant", null, null },
                    { 4L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "B-", null, 1, "default_tenant", null, null },
                    { 5L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "AB+", null, 1, "default_tenant", null, null },
                    { 6L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "AB-", null, 1, "default_tenant", null, null },
                    { 7L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "O+", null, 1, "default_tenant", null, null },
                    { 8L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "O-", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DialCode", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, "BD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+880", "Bangladesh", null, 1, "default_tenant", null, null },
                    { 2L, "IN", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+91", "India", null, 1, "default_tenant", null, null },
                    { 3L, "PK", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+92", "Pakistan", null, 1, "default_tenant", null, null },
                    { 4L, "LK", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+94", "Sri Lanka", null, 1, "default_tenant", null, null },
                    { 5L, "NP", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+977", "Nepal", null, 1, "default_tenant", null, null },
                    { 6L, "BT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+975", "Bhutan", null, 1, "default_tenant", null, null },
                    { 7L, "MM", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+95", "Myanmar", null, 1, "default_tenant", null, null },
                    { 8L, "AF", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+93", "Afghanistan", null, 1, "default_tenant", null, null },
                    { 9L, "MV", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+960", "Maldives", null, 1, "default_tenant", null, null },
                    { 10L, "CN", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+86", "China", null, 1, "default_tenant", null, null },
                    { 11L, "JP", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+81", "Japan", null, 1, "default_tenant", null, null },
                    { 12L, "KR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+82", "South Korea", null, 1, "default_tenant", null, null },
                    { 13L, "KP", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+850", "North Korea", null, 1, "default_tenant", null, null },
                    { 14L, "MN", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+976", "Mongolia", null, 1, "default_tenant", null, null },
                    { 15L, "TW", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+886", "Taiwan", null, 1, "default_tenant", null, null },
                    { 16L, "HK", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+852", "Hong Kong", null, 1, "default_tenant", null, null },
                    { 17L, "MO", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+853", "Macau", null, 1, "default_tenant", null, null },
                    { 18L, "VN", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+84", "Vietnam", null, 1, "default_tenant", null, null },
                    { 19L, "TH", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+66", "Thailand", null, 1, "default_tenant", null, null },
                    { 20L, "KH", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+855", "Cambodia", null, 1, "default_tenant", null, null },
                    { 21L, "LA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+856", "Laos", null, 1, "default_tenant", null, null },
                    { 22L, "MY", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+60", "Malaysia", null, 1, "default_tenant", null, null },
                    { 23L, "SG", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+65", "Singapore", null, 1, "default_tenant", null, null },
                    { 24L, "ID", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+62", "Indonesia", null, 1, "default_tenant", null, null },
                    { 25L, "PH", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+63", "Philippines", null, 1, "default_tenant", null, null },
                    { 26L, "BN", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+673", "Brunei", null, 1, "default_tenant", null, null },
                    { 27L, "TL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+670", "Timor-Leste", null, 1, "default_tenant", null, null },
                    { 28L, "SA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+966", "Saudi Arabia", null, 1, "default_tenant", null, null },
                    { 29L, "AE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+971", "United Arab Emirates", null, 1, "default_tenant", null, null },
                    { 30L, "QA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+974", "Qatar", null, 1, "default_tenant", null, null },
                    { 31L, "KW", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+965", "Kuwait", null, 1, "default_tenant", null, null },
                    { 32L, "BH", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+973", "Bahrain", null, 1, "default_tenant", null, null },
                    { 33L, "OM", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+968", "Oman", null, 1, "default_tenant", null, null },
                    { 34L, "YE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+967", "Yemen", null, 1, "default_tenant", null, null },
                    { 35L, "IQ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+964", "Iraq", null, 1, "default_tenant", null, null },
                    { 36L, "IR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+98", "Iran", null, 1, "default_tenant", null, null },
                    { 37L, "IL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+972", "Israel", null, 1, "default_tenant", null, null },
                    { 38L, "PS", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+970", "Palestine", null, 1, "default_tenant", null, null },
                    { 39L, "JO", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+962", "Jordan", null, 1, "default_tenant", null, null },
                    { 40L, "LB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+961", "Lebanon", null, 1, "default_tenant", null, null },
                    { 41L, "SY", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+963", "Syria", null, 1, "default_tenant", null, null },
                    { 42L, "TR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+90", "Turkey", null, 1, "default_tenant", null, null },
                    { 43L, "CY", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+357", "Cyprus", null, 1, "default_tenant", null, null },
                    { 44L, "KZ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+7", "Kazakhstan", null, 1, "default_tenant", null, null },
                    { 45L, "UZ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+998", "Uzbekistan", null, 1, "default_tenant", null, null },
                    { 46L, "TM", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+993", "Turkmenistan", null, 1, "default_tenant", null, null },
                    { 47L, "TJ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+992", "Tajikistan", null, 1, "default_tenant", null, null },
                    { 48L, "KG", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+996", "Kyrgyzstan", null, 1, "default_tenant", null, null },
                    { 49L, "AZ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+994", "Azerbaijan", null, 1, "default_tenant", null, null },
                    { 50L, "AM", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+374", "Armenia", null, 1, "default_tenant", null, null },
                    { 51L, "GE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+995", "Georgia", null, 1, "default_tenant", null, null },
                    { 52L, "RU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+7", "Russia", null, 1, "default_tenant", null, null },
                    { 53L, "UA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+380", "Ukraine", null, 1, "default_tenant", null, null },
                    { 54L, "BY", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+375", "Belarus", null, 1, "default_tenant", null, null },
                    { 55L, "MD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+373", "Moldova", null, 1, "default_tenant", null, null },
                    { 56L, "PL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+48", "Poland", null, 1, "default_tenant", null, null },
                    { 57L, "CZ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+420", "Czech Republic", null, 1, "default_tenant", null, null },
                    { 58L, "SK", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+421", "Slovakia", null, 1, "default_tenant", null, null },
                    { 59L, "HU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+36", "Hungary", null, 1, "default_tenant", null, null },
                    { 60L, "RO", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+40", "Romania", null, 1, "default_tenant", null, null },
                    { 61L, "BG", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+359", "Bulgaria", null, 1, "default_tenant", null, null },
                    { 62L, "RS", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+381", "Serbia", null, 1, "default_tenant", null, null },
                    { 63L, "HR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+385", "Croatia", null, 1, "default_tenant", null, null },
                    { 64L, "SI", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+386", "Slovenia", null, 1, "default_tenant", null, null },
                    { 65L, "BA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+387", "Bosnia and Herzegovina", null, 1, "default_tenant", null, null },
                    { 66L, "ME", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+382", "Montenegro", null, 1, "default_tenant", null, null },
                    { 67L, "MK", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+389", "North Macedonia", null, 1, "default_tenant", null, null },
                    { 68L, "AL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+355", "Albania", null, 1, "default_tenant", null, null },
                    { 69L, "XK", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+383", "Kosovo", null, 1, "default_tenant", null, null },
                    { 70L, "GR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+30", "Greece", null, 1, "default_tenant", null, null },
                    { 71L, "IT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+39", "Italy", null, 1, "default_tenant", null, null },
                    { 72L, "MT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+356", "Malta", null, 1, "default_tenant", null, null },
                    { 73L, "VA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+379", "Vatican City", null, 1, "default_tenant", null, null },
                    { 74L, "SM", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+378", "San Marino", null, 1, "default_tenant", null, null },
                    { 75L, "ES", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+34", "Spain", null, 1, "default_tenant", null, null },
                    { 76L, "PT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+351", "Portugal", null, 1, "default_tenant", null, null },
                    { 77L, "AD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+376", "Andorra", null, 1, "default_tenant", null, null },
                    { 78L, "FR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+33", "France", null, 1, "default_tenant", null, null },
                    { 79L, "MC", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+377", "Monaco", null, 1, "default_tenant", null, null },
                    { 80L, "DE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+49", "Germany", null, 1, "default_tenant", null, null },
                    { 81L, "AT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+43", "Austria", null, 1, "default_tenant", null, null },
                    { 82L, "CH", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+41", "Switzerland", null, 1, "default_tenant", null, null },
                    { 83L, "LI", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+423", "Liechtenstein", null, 1, "default_tenant", null, null },
                    { 84L, "BE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+32", "Belgium", null, 1, "default_tenant", null, null },
                    { 85L, "NL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+31", "Netherlands", null, 1, "default_tenant", null, null },
                    { 86L, "LU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+352", "Luxembourg", null, 1, "default_tenant", null, null },
                    { 87L, "GB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+44", "United Kingdom", null, 1, "default_tenant", null, null },
                    { 88L, "IE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+353", "Ireland", null, 1, "default_tenant", null, null },
                    { 89L, "IS", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+354", "Iceland", null, 1, "default_tenant", null, null },
                    { 90L, "DK", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+45", "Denmark", null, 1, "default_tenant", null, null },
                    { 91L, "NO", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+47", "Norway", null, 1, "default_tenant", null, null },
                    { 92L, "SE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+46", "Sweden", null, 1, "default_tenant", null, null },
                    { 93L, "FI", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+358", "Finland", null, 1, "default_tenant", null, null },
                    { 94L, "EE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+372", "Estonia", null, 1, "default_tenant", null, null },
                    { 95L, "LV", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+371", "Latvia", null, 1, "default_tenant", null, null },
                    { 96L, "LT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+370", "Lithuania", null, 1, "default_tenant", null, null },
                    { 97L, "US", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+1", "United States", null, 1, "default_tenant", null, null },
                    { 98L, "CA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+1", "Canada", null, 1, "default_tenant", null, null },
                    { 99L, "MX", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+52", "Mexico", null, 1, "default_tenant", null, null },
                    { 100L, "GT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+502", "Guatemala", null, 1, "default_tenant", null, null },
                    { 101L, "BZ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+501", "Belize", null, 1, "default_tenant", null, null },
                    { 102L, "SV", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+503", "El Salvador", null, 1, "default_tenant", null, null },
                    { 103L, "HN", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+504", "Honduras", null, 1, "default_tenant", null, null },
                    { 104L, "NI", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+505", "Nicaragua", null, 1, "default_tenant", null, null },
                    { 105L, "CR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+506", "Costa Rica", null, 1, "default_tenant", null, null },
                    { 106L, "PA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+507", "Panama", null, 1, "default_tenant", null, null },
                    { 107L, "CU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+53", "Cuba", null, 1, "default_tenant", null, null },
                    { 108L, "JM", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+1876", "Jamaica", null, 1, "default_tenant", null, null },
                    { 109L, "HT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+509", "Haiti", null, 1, "default_tenant", null, null },
                    { 110L, "DO", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+1809", "Dominican Republic", null, 1, "default_tenant", null, null },
                    { 111L, "TT", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+1868", "Trinidad and Tobago", null, 1, "default_tenant", null, null },
                    { 112L, "BS", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+1242", "Bahamas", null, 1, "default_tenant", null, null },
                    { 113L, "BB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+1246", "Barbados", null, 1, "default_tenant", null, null },
                    { 114L, "BR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+55", "Brazil", null, 1, "default_tenant", null, null },
                    { 115L, "AR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+54", "Argentina", null, 1, "default_tenant", null, null },
                    { 116L, "CL", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+56", "Chile", null, 1, "default_tenant", null, null },
                    { 117L, "PE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+51", "Peru", null, 1, "default_tenant", null, null },
                    { 118L, "CO", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+57", "Colombia", null, 1, "default_tenant", null, null },
                    { 119L, "VE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+58", "Venezuela", null, 1, "default_tenant", null, null },
                    { 120L, "EC", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+593", "Ecuador", null, 1, "default_tenant", null, null },
                    { 121L, "BO", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+591", "Bolivia", null, 1, "default_tenant", null, null },
                    { 122L, "PY", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+595", "Paraguay", null, 1, "default_tenant", null, null },
                    { 123L, "UY", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+598", "Uruguay", null, 1, "default_tenant", null, null },
                    { 124L, "GY", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+592", "Guyana", null, 1, "default_tenant", null, null },
                    { 125L, "SR", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+597", "Suriname", null, 1, "default_tenant", null, null },
                    { 126L, "EG", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+20", "Egypt", null, 1, "default_tenant", null, null },
                    { 127L, "LY", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+218", "Libya", null, 1, "default_tenant", null, null },
                    { 128L, "TN", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+216", "Tunisia", null, 1, "default_tenant", null, null },
                    { 129L, "DZ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+213", "Algeria", null, 1, "default_tenant", null, null },
                    { 130L, "MA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+212", "Morocco", null, 1, "default_tenant", null, null },
                    { 131L, "SD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+249", "Sudan", null, 1, "default_tenant", null, null },
                    { 132L, "SS", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+211", "South Sudan", null, 1, "default_tenant", null, null },
                    { 133L, "ET", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+251", "Ethiopia", null, 1, "default_tenant", null, null },
                    { 134L, "ER", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+291", "Eritrea", null, 1, "default_tenant", null, null },
                    { 135L, "DJ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+253", "Djibouti", null, 1, "default_tenant", null, null },
                    { 136L, "SO", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+252", "Somalia", null, 1, "default_tenant", null, null },
                    { 137L, "KE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+254", "Kenya", null, 1, "default_tenant", null, null },
                    { 138L, "UG", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+256", "Uganda", null, 1, "default_tenant", null, null },
                    { 139L, "TZ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+255", "Tanzania", null, 1, "default_tenant", null, null },
                    { 140L, "RW", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+250", "Rwanda", null, 1, "default_tenant", null, null },
                    { 141L, "BI", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+257", "Burundi", null, 1, "default_tenant", null, null },
                    { 142L, "NG", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+234", "Nigeria", null, 1, "default_tenant", null, null },
                    { 143L, "GH", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+233", "Ghana", null, 1, "default_tenant", null, null },
                    { 144L, "CI", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+225", "Ivory Coast", null, 1, "default_tenant", null, null },
                    { 145L, "SN", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+221", "Senegal", null, 1, "default_tenant", null, null },
                    { 146L, "ML", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+223", "Mali", null, 1, "default_tenant", null, null },
                    { 147L, "NE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+227", "Niger", null, 1, "default_tenant", null, null },
                    { 148L, "TD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+235", "Chad", null, 1, "default_tenant", null, null },
                    { 149L, "CM", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+237", "Cameroon", null, 1, "default_tenant", null, null },
                    { 150L, "GA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+241", "Gabon", null, 1, "default_tenant", null, null },
                    { 151L, "CG", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+242", "Congo", null, 1, "default_tenant", null, null },
                    { 152L, "CD", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+243", "DR Congo", null, 1, "default_tenant", null, null },
                    { 153L, "AO", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+244", "Angola", null, 1, "default_tenant", null, null },
                    { 154L, "ZM", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+260", "Zambia", null, 1, "default_tenant", null, null },
                    { 155L, "ZW", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+263", "Zimbabwe", null, 1, "default_tenant", null, null },
                    { 156L, "MW", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+265", "Malawi", null, 1, "default_tenant", null, null },
                    { 157L, "MZ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+258", "Mozambique", null, 1, "default_tenant", null, null },
                    { 158L, "NA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+264", "Namibia", null, 1, "default_tenant", null, null },
                    { 159L, "BW", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+267", "Botswana", null, 1, "default_tenant", null, null },
                    { 160L, "ZA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+27", "South Africa", null, 1, "default_tenant", null, null },
                    { 161L, "LS", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+266", "Lesotho", null, 1, "default_tenant", null, null },
                    { 162L, "SZ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+268", "Eswatini", null, 1, "default_tenant", null, null },
                    { 163L, "MG", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+261", "Madagascar", null, 1, "default_tenant", null, null },
                    { 164L, "MU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+230", "Mauritius", null, 1, "default_tenant", null, null },
                    { 165L, "SC", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+248", "Seychelles", null, 1, "default_tenant", null, null },
                    { 166L, "AU", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+61", "Australia", null, 1, "default_tenant", null, null },
                    { 167L, "NZ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+64", "New Zealand", null, 1, "default_tenant", null, null },
                    { 168L, "FJ", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+679", "Fiji", null, 1, "default_tenant", null, null },
                    { 169L, "PG", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "+675", "Papua New Guinea", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.InsertData(
                table: "Degrees",
                columns: new[] { "DegreeId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "FullName", "Name", "Position", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Below SSC", "Below SSC", 1, null, 1, "default_tenant", null, null },
                    { 2L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Dakhil", "Dakhil", 1, null, 1, "default_tenant", null, null },
                    { 3L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Ordinary Level", "O Level", 1, null, 1, "default_tenant", null, null },
                    { 4L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Secondary School Certificate", "SSC", 1, null, 1, "default_tenant", null, null },
                    { 5L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Advance Level", "A Level", 2, null, 1, "default_tenant", null, null },
                    { 6L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Alim", "Alim", 2, null, 1, "default_tenant", null, null },
                    { 7L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Diploma in Commerce", "Diploma (Comm)", 2, null, 1, "default_tenant", null, null },
                    { 8L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Diploma-in-Engineering", "Diploma (Engg)", 2, null, 1, "default_tenant", null, null },
                    { 9L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "The General Education", "GED", 2, null, 1, "default_tenant", null, null },
                    { 10L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Higher Secondary School Certificate", "HSC", 2, null, 1, "default_tenant", null, null },
                    { 11L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Science in Agricultural Engineering", "B.Sc. Agril. Engg.", 3, null, 1, "default_tenant", null, null },
                    { 12L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Science in Food Engineering", "B.Sc. Food Engg.", 3, null, 1, "default_tenant", null, null },
                    { 13L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Arts (Honours)", "BA (Hons)", 3, null, 1, "default_tenant", null, null },
                    { 14L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Arts (Pass)", "BA (Pass)", 3, null, 1, "default_tenant", null, null },
                    { 15L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Architecture", "BArch", 3, null, 1, "default_tenant", null, null },
                    { 16L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Business Administration", "BBA", 3, null, 1, "default_tenant", null, null },
                    { 17L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Business Administration", "BBA (Hons)", 3, null, 1, "default_tenant", null, null },
                    { 18L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Bank Management", "BBM", 3, null, 1, "default_tenant", null, null },
                    { 19L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Business Studies (Honours)", "BBS (Hons)", 3, null, 1, "default_tenant", null, null },
                    { 20L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Business Studies (Pass)", "BBS (Pass)", 3, null, 1, "default_tenant", null, null },
                    { 21L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Computer Application", "BCA (Hons)", 3, null, 1, "default_tenant", null, null },
                    { 22L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Commerce (Honours)", "BCom (Hons)", 3, null, 1, "default_tenant", null, null },
                    { 23L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Commerce (Pass)", "BCom (Pass)", 3, null, 1, "default_tenant", null, null },
                    { 24L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Education", "BEd", 3, null, 1, "default_tenant", null, null },
                    { 25L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Social Science (Honours)", "BSS (Hons)", 3, null, 1, "default_tenant", null, null },
                    { 26L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Social Science (Pass)", "BSS (Pass)", 3, null, 1, "default_tenant", null, null },
                    { 27L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Science (Engineering)", "BSc (Engg)", 3, null, 1, "default_tenant", null, null },
                    { 28L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Science (Honours)", "BSc (Hons)", 3, null, 1, "default_tenant", null, null },
                    { 29L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Science (Pass)", "BSc (Pass)", 3, null, 1, "default_tenant", null, null },
                    { 30L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Technology", "BTech", 3, null, 1, "default_tenant", null, null },
                    { 31L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Fazil", "Fazil", 3, null, 1, "default_tenant", null, null },
                    { 32L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Law", "LLB", 3, null, 1, "default_tenant", null, null },
                    { 33L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bachelor of Medicine, Bachelor of Surgery", "MBBS", 3, null, 1, "default_tenant", null, null },
                    { 34L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Kamil", "Kamil", 4, null, 1, "default_tenant", null, null },
                    { 35L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Master of Law", "LLM", 4, null, 1, "default_tenant", null, null },
                    { 36L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Master of Arts", "MA", 4, null, 1, "default_tenant", null, null },
                    { 37L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Master of Business Administration", "MBA", 4, null, 1, "default_tenant", null, null },
                    { 38L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Master of Banking Management", "MBM", 4, null, 1, "default_tenant", null, null },
                    { 39L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Master of Business Studies", "MBS", 4, null, 1, "default_tenant", null, null },
                    { 40L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Master of Commerce", "MCom", 4, null, 1, "default_tenant", null, null },
                    { 41L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Master of Education", "MEd", 4, null, 1, "default_tenant", null, null },
                    { 42L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Master in Information Technology", "MIT", 4, null, 1, "default_tenant", null, null },
                    { 43L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Master of Science", "MS", 4, null, 1, "default_tenant", null, null }
                });

            migrationBuilder.InsertData(
                table: "EducationBoards",
                columns: new[] { "EducationBoardId", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, "B001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Dhaka Board", null, 1, "default_tenant", null, null },
                    { 2L, "B002", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Chittagong Board", null, 1, "default_tenant", null, null },
                    { 3L, "B003", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Barisal Board", null, 1, "default_tenant", null, null },
                    { 4L, "B004", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Jessore Board", null, 1, "default_tenant", null, null },
                    { 5L, "B005", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Rajshahi Board", null, 1, "default_tenant", null, null },
                    { 6L, "B006", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Comilla Board", null, 1, "default_tenant", null, null },
                    { 7L, "B007", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Dinajpur Board", null, 1, "default_tenant", null, null },
                    { 8L, "B008", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Sylhet Board", null, 1, "default_tenant", null, null },
                    { 9L, "B009", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Madrasah Board", null, 1, "default_tenant", null, null },
                    { 10L, "B010", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Technical Education Board", null, 1, "default_tenant", null, null },
                    { 11L, "B011", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Edexcel", null, 1, "default_tenant", null, null },
                    { 12L, "B012", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Others for Foreign Country", null, 1, "default_tenant", null, null },
                    { 13L, "B013", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Bangladesh Open University", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "GenderId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Male", null, 1, "default_tenant", null, null },
                    { 2L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Female", null, 1, "default_tenant", null, null },
                    { 3L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Other", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.InsertData(
                table: "MaritalStatuses",
                columns: new[] { "MaritalStatusId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Single", null, 1, "default_tenant", null, null },
                    { 2L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Married", null, 1, "default_tenant", null, null },
                    { 3L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Other", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.InsertData(
                table: "Religions",
                columns: new[] { "ReligionId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Islam", null, 1, "default_tenant", null, null },
                    { 2L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Hinduism", null, 1, "default_tenant", null, null },
                    { 3L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Christianity", null, 1, "default_tenant", null, null },
                    { 4L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Buddhism", null, 1, "default_tenant", null, null },
                    { 5L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Other", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_BloodGroupId",
                table: "CandidateProfiles",
                column: "BloodGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_CountryId",
                table: "CandidateProfiles",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_GenderId",
                table: "CandidateProfiles",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_MaritalStatusId",
                table: "CandidateProfiles",
                column: "MaritalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_ReligionId",
                table: "CandidateProfiles",
                column: "ReligionId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEducations_DegreeId",
                table: "CandidateEducations",
                column: "DegreeId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEducations_EducationBoardId",
                table: "CandidateEducations",
                column: "EducationBoardId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodGroups_Name",
                table: "BloodGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Code",
                table: "Countries",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Degrees_Name",
                table: "Degrees",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationBoards_Name",
                table: "EducationBoards",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genders_Name",
                table: "Genders",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaritalStatuses_Name",
                table: "MaritalStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Religions_Name",
                table: "Religions",
                column: "Name",
                unique: true);

            // DegreeId is a new required column (default 0 on pre-existing rows) but DegreeTitle
            // free text is being dropped in this same migration, so there's nothing left to
            // resolve it against - backfill to Degree 1 ("Below SSC") as a safe placeholder the
            // candidate corrects on their next profile edit, rather than leaving an invalid FK.
            migrationBuilder.Sql(
                "UPDATE \"CandidateEducations\" SET \"DegreeId\" = 1 WHERE \"DegreeId\" NOT IN (SELECT \"DegreeId\" FROM \"Degrees\");");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateEducations_Degrees_DegreeId",
                table: "CandidateEducations",
                column: "DegreeId",
                principalTable: "Degrees",
                principalColumn: "DegreeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateEducations_EducationBoards_EducationBoardId",
                table: "CandidateEducations",
                column: "EducationBoardId",
                principalTable: "EducationBoards",
                principalColumn: "EducationBoardId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_BloodGroups_BloodGroupId",
                table: "CandidateProfiles",
                column: "BloodGroupId",
                principalTable: "BloodGroups",
                principalColumn: "BloodGroupId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_Countries_CountryId",
                table: "CandidateProfiles",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_Genders_GenderId",
                table: "CandidateProfiles",
                column: "GenderId",
                principalTable: "Genders",
                principalColumn: "GenderId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_MaritalStatuses_MaritalStatusId",
                table: "CandidateProfiles",
                column: "MaritalStatusId",
                principalTable: "MaritalStatuses",
                principalColumn: "MaritalStatusId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_Religions_ReligionId",
                table: "CandidateProfiles",
                column: "ReligionId",
                principalTable: "Religions",
                principalColumn: "ReligionId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateEducations_Degrees_DegreeId",
                table: "CandidateEducations");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateEducations_EducationBoards_EducationBoardId",
                table: "CandidateEducations");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_BloodGroups_BloodGroupId",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Countries_CountryId",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Genders_GenderId",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_MaritalStatuses_MaritalStatusId",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Religions_ReligionId",
                table: "CandidateProfiles");

            migrationBuilder.DropTable(
                name: "BloodGroups");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Degrees");

            migrationBuilder.DropTable(
                name: "EducationBoards");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "MaritalStatuses");

            migrationBuilder.DropTable(
                name: "Religions");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_BloodGroupId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_CountryId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_GenderId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_MaritalStatusId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_ReligionId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateEducations_DegreeId",
                table: "CandidateEducations");

            migrationBuilder.DropIndex(
                name: "IX_CandidateEducations_EducationBoardId",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "BloodGroupId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "GenderId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "MaritalStatusId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "ReligionId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "DegreeId",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "EducationBoardId",
                table: "CandidateEducations");

            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "CandidateProfiles",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "CandidateProfiles",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "CandidateProfiles",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileOperator",
                table: "CandidateProfiles",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Religion",
                table: "CandidateProfiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegreeTitle",
                table: "CandidateEducations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
