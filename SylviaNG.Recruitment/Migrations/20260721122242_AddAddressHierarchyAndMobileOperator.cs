using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressHierarchyAndMobileOperator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PresentAddress",
                table: "CandidateProfiles",
                newName: "PresentAddressDetail");

            migrationBuilder.RenameColumn(
                name: "PermanentAddress",
                table: "CandidateProfiles",
                newName: "PermanentAddressDetail");

            migrationBuilder.AddColumn<long>(
                name: "HomeDistrictId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HomeDivisionId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HomeThanaId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileOperator",
                table: "CandidateProfiles",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PresentDistrictId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PresentDivisionId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PresentThanaId",
                table: "CandidateProfiles",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    DivisionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_Divisions", x => x.DivisionId);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    DistrictId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DivisionId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Districts", x => x.DistrictId);
                    table.ForeignKey(
                        name: "FK_Districts_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "DivisionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Thanas",
                columns: table => new
                {
                    ThanaId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DistrictId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Thanas", x => x.ThanaId);
                    table.ForeignKey(
                        name: "FK_Thanas_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "DistrictId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Divisions",
                columns: new[] { "DivisionId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Barisal", null, 1, "default_tenant", null, null },
                    { 2L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Chattogram", null, 1, "default_tenant", null, null },
                    { 3L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Dhaka", null, 1, "default_tenant", null, null },
                    { 4L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Khulna", null, 1, "default_tenant", null, null },
                    { 5L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Mymensingh", null, 1, "default_tenant", null, null },
                    { 6L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Rajshahi", null, 1, "default_tenant", null, null },
                    { 7L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Rangpur", null, 1, "default_tenant", null, null },
                    { 8L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, "Sylhet", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.InsertData(
                table: "Districts",
                columns: new[] { "DistrictId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DivisionId", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Barisal", null, 1, "default_tenant", null, null },
                    { 2L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Barguna", null, 1, "default_tenant", null, null },
                    { 3L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Bhola", null, 1, "default_tenant", null, null },
                    { 4L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Jhalokati", null, 1, "default_tenant", null, null },
                    { 5L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Patuakhali", null, 1, "default_tenant", null, null },
                    { 6L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Pirojpur", null, 1, "default_tenant", null, null },
                    { 7L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Bandarban", null, 1, "default_tenant", null, null },
                    { 8L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Brahmanbaria", null, 1, "default_tenant", null, null },
                    { 9L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Chandpur", null, 1, "default_tenant", null, null },
                    { 10L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Chattogram", null, 1, "default_tenant", null, null },
                    { 11L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Cumilla", null, 1, "default_tenant", null, null },
                    { 12L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Cox's Bazar", null, 1, "default_tenant", null, null },
                    { 13L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Feni", null, 1, "default_tenant", null, null },
                    { 14L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Khagrachhari", null, 1, "default_tenant", null, null },
                    { 15L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Lakshmipur", null, 1, "default_tenant", null, null },
                    { 16L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Noakhali", null, 1, "default_tenant", null, null },
                    { 17L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Rangamati", null, 1, "default_tenant", null, null },
                    { 18L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Dhaka", null, 1, "default_tenant", null, null },
                    { 19L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Faridpur", null, 1, "default_tenant", null, null },
                    { 20L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Gazipur", null, 1, "default_tenant", null, null },
                    { 21L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Gopalganj", null, 1, "default_tenant", null, null },
                    { 22L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Kishoreganj", null, 1, "default_tenant", null, null },
                    { 23L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Madaripur", null, 1, "default_tenant", null, null },
                    { 24L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Manikganj", null, 1, "default_tenant", null, null },
                    { 25L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Munshiganj", null, 1, "default_tenant", null, null },
                    { 26L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Narayanganj", null, 1, "default_tenant", null, null },
                    { 27L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Narsingdi", null, 1, "default_tenant", null, null },
                    { 28L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Rajbari", null, 1, "default_tenant", null, null },
                    { 29L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Shariatpur", null, 1, "default_tenant", null, null },
                    { 30L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Tangail", null, 1, "default_tenant", null, null },
                    { 31L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Bagerhat", null, 1, "default_tenant", null, null },
                    { 32L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Chuadanga", null, 1, "default_tenant", null, null },
                    { 33L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Jashore", null, 1, "default_tenant", null, null },
                    { 34L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Jhenaidah", null, 1, "default_tenant", null, null },
                    { 35L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Khulna", null, 1, "default_tenant", null, null },
                    { 36L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Kushtia", null, 1, "default_tenant", null, null },
                    { 37L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Magura", null, 1, "default_tenant", null, null },
                    { 38L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Meherpur", null, 1, "default_tenant", null, null },
                    { 39L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Narail", null, 1, "default_tenant", null, null },
                    { 40L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Satkhira", null, 1, "default_tenant", null, null },
                    { 41L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 5L, "Jamalpur", null, 1, "default_tenant", null, null },
                    { 42L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 5L, "Mymensingh", null, 1, "default_tenant", null, null },
                    { 43L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 5L, "Netrokona", null, 1, "default_tenant", null, null },
                    { 44L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 5L, "Sherpur", null, 1, "default_tenant", null, null },
                    { 45L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Bogura", null, 1, "default_tenant", null, null },
                    { 46L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Chapainawabganj", null, 1, "default_tenant", null, null },
                    { 47L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Joypurhat", null, 1, "default_tenant", null, null },
                    { 48L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Naogaon", null, 1, "default_tenant", null, null },
                    { 49L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Natore", null, 1, "default_tenant", null, null },
                    { 50L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Pabna", null, 1, "default_tenant", null, null },
                    { 51L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Rajshahi", null, 1, "default_tenant", null, null },
                    { 52L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Sirajganj", null, 1, "default_tenant", null, null },
                    { 53L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Dinajpur", null, 1, "default_tenant", null, null },
                    { 54L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Gaibandha", null, 1, "default_tenant", null, null },
                    { 55L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Kurigram", null, 1, "default_tenant", null, null },
                    { 56L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Lalmonirhat", null, 1, "default_tenant", null, null },
                    { 57L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Nilphamari", null, 1, "default_tenant", null, null },
                    { 58L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Panchagarh", null, 1, "default_tenant", null, null },
                    { 59L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Rangpur", null, 1, "default_tenant", null, null },
                    { 60L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Thakurgaon", null, 1, "default_tenant", null, null },
                    { 61L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 8L, "Habiganj", null, 1, "default_tenant", null, null },
                    { 62L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 8L, "Moulvibazar", null, 1, "default_tenant", null, null },
                    { 63L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 8L, "Sunamganj", null, 1, "default_tenant", null, null },
                    { 64L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 8L, "Sylhet", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.InsertData(
                table: "Thanas",
                columns: new[] { "ThanaId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DistrictId", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Barisal Sadar", null, 1, "default_tenant", null, null },
                    { 2L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Bakerganj", null, 1, "default_tenant", null, null },
                    { 3L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Babuganj", null, 1, "default_tenant", null, null },
                    { 4L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Banaripara", null, 1, "default_tenant", null, null },
                    { 5L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Gaurnadi", null, 1, "default_tenant", null, null },
                    { 6L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Agailjhara", null, 1, "default_tenant", null, null },
                    { 7L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Mehendiganj", null, 1, "default_tenant", null, null },
                    { 8L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Muladi", null, 1, "default_tenant", null, null },
                    { 9L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Wazirpur", null, 1, "default_tenant", null, null },
                    { 10L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Barguna Sadar", null, 1, "default_tenant", null, null },
                    { 11L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Amtali", null, 1, "default_tenant", null, null },
                    { 12L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Bamna", null, 1, "default_tenant", null, null },
                    { 13L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Betagi", null, 1, "default_tenant", null, null },
                    { 14L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Patharghata", null, 1, "default_tenant", null, null },
                    { 15L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 2L, "Taltali", null, 1, "default_tenant", null, null },
                    { 16L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Bhola Sadar", null, 1, "default_tenant", null, null },
                    { 17L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Burhanuddin", null, 1, "default_tenant", null, null },
                    { 18L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Char Fasson", null, 1, "default_tenant", null, null },
                    { 19L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Daulatkhan", null, 1, "default_tenant", null, null },
                    { 20L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Lalmohan", null, 1, "default_tenant", null, null },
                    { 21L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Manpura", null, 1, "default_tenant", null, null },
                    { 22L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 3L, "Tazumuddin", null, 1, "default_tenant", null, null },
                    { 23L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Jhalokati Sadar", null, 1, "default_tenant", null, null },
                    { 24L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Kathalia", null, 1, "default_tenant", null, null },
                    { 25L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Nalchity", null, 1, "default_tenant", null, null },
                    { 26L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 4L, "Rajapur", null, 1, "default_tenant", null, null },
                    { 27L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 5L, "Patuakhali Sadar", null, 1, "default_tenant", null, null },
                    { 28L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 5L, "Bauphal", null, 1, "default_tenant", null, null },
                    { 29L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 5L, "Dashmina", null, 1, "default_tenant", null, null },
                    { 30L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 5L, "Dumki", null, 1, "default_tenant", null, null },
                    { 31L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 5L, "Galachipa", null, 1, "default_tenant", null, null },
                    { 32L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 5L, "Kalapara", null, 1, "default_tenant", null, null },
                    { 33L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 5L, "Mirzaganj", null, 1, "default_tenant", null, null },
                    { 34L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 5L, "Rangabali", null, 1, "default_tenant", null, null },
                    { 35L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Pirojpur Sadar", null, 1, "default_tenant", null, null },
                    { 36L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Bhandaria", null, 1, "default_tenant", null, null },
                    { 37L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Kawkhali", null, 1, "default_tenant", null, null },
                    { 38L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Mathbaria", null, 1, "default_tenant", null, null },
                    { 39L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Nazirpur", null, 1, "default_tenant", null, null },
                    { 40L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Nesarabad", null, 1, "default_tenant", null, null },
                    { 41L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 6L, "Zianagar", null, 1, "default_tenant", null, null },
                    { 42L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Bandarban Sadar", null, 1, "default_tenant", null, null },
                    { 43L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Alikadam", null, 1, "default_tenant", null, null },
                    { 44L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Lama", null, 1, "default_tenant", null, null },
                    { 45L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Naikhongchhari", null, 1, "default_tenant", null, null },
                    { 46L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Rowangchhari", null, 1, "default_tenant", null, null },
                    { 47L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Ruma", null, 1, "default_tenant", null, null },
                    { 48L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 7L, "Thanchi", null, 1, "default_tenant", null, null },
                    { 49L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 8L, "Brahmanbaria Sadar", null, 1, "default_tenant", null, null },
                    { 50L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 8L, "Akhaura", null, 1, "default_tenant", null, null },
                    { 51L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 8L, "Bancharampur", null, 1, "default_tenant", null, null },
                    { 52L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 8L, "Bijoynagar", null, 1, "default_tenant", null, null },
                    { 53L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 8L, "Kasba", null, 1, "default_tenant", null, null },
                    { 54L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 8L, "Nabinagar", null, 1, "default_tenant", null, null },
                    { 55L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 8L, "Nasirnagar", null, 1, "default_tenant", null, null },
                    { 56L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 8L, "Sarail", null, 1, "default_tenant", null, null },
                    { 57L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 9L, "Chandpur Sadar", null, 1, "default_tenant", null, null },
                    { 58L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 9L, "Faridganj", null, 1, "default_tenant", null, null },
                    { 59L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 9L, "Haimchar", null, 1, "default_tenant", null, null },
                    { 60L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 9L, "Hajiganj", null, 1, "default_tenant", null, null },
                    { 61L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 9L, "Kachua", null, 1, "default_tenant", null, null },
                    { 62L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 9L, "Matlab Dakshin", null, 1, "default_tenant", null, null },
                    { 63L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 9L, "Matlab Uttar", null, 1, "default_tenant", null, null },
                    { 64L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 9L, "Shahrasti", null, 1, "default_tenant", null, null },
                    { 65L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Anwara", null, 1, "default_tenant", null, null },
                    { 66L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Banshkhali", null, 1, "default_tenant", null, null },
                    { 67L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Boalkhali", null, 1, "default_tenant", null, null },
                    { 68L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Chandanaish", null, 1, "default_tenant", null, null },
                    { 69L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Fatikchhari", null, 1, "default_tenant", null, null },
                    { 70L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Hathazari", null, 1, "default_tenant", null, null },
                    { 71L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Karnaphuli", null, 1, "default_tenant", null, null },
                    { 72L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Lohagara", null, 1, "default_tenant", null, null },
                    { 73L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Mirsharai", null, 1, "default_tenant", null, null },
                    { 74L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Patiya", null, 1, "default_tenant", null, null },
                    { 75L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Rangunia", null, 1, "default_tenant", null, null },
                    { 76L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Raozan", null, 1, "default_tenant", null, null },
                    { 77L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Sandwip", null, 1, "default_tenant", null, null },
                    { 78L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Satkania", null, 1, "default_tenant", null, null },
                    { 79L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Sitakunda", null, 1, "default_tenant", null, null },
                    { 80L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Cumilla Sadar", null, 1, "default_tenant", null, null },
                    { 81L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Barura", null, 1, "default_tenant", null, null },
                    { 82L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Brahmanpara", null, 1, "default_tenant", null, null },
                    { 83L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Burichang", null, 1, "default_tenant", null, null },
                    { 84L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Chandina", null, 1, "default_tenant", null, null },
                    { 85L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Chauddagram", null, 1, "default_tenant", null, null },
                    { 86L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Daudkandi", null, 1, "default_tenant", null, null },
                    { 87L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Debidwar", null, 1, "default_tenant", null, null },
                    { 88L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Homna", null, 1, "default_tenant", null, null },
                    { 89L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Laksam", null, 1, "default_tenant", null, null },
                    { 90L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Meghna", null, 1, "default_tenant", null, null },
                    { 91L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Monohorgonj", null, 1, "default_tenant", null, null },
                    { 92L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Muradnagar", null, 1, "default_tenant", null, null },
                    { 93L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Nangalkot", null, 1, "default_tenant", null, null },
                    { 94L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 11L, "Titas", null, 1, "default_tenant", null, null },
                    { 95L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 12L, "Cox's Bazar Sadar", null, 1, "default_tenant", null, null },
                    { 96L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 12L, "Chakaria", null, 1, "default_tenant", null, null },
                    { 97L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 12L, "Kutubdia", null, 1, "default_tenant", null, null },
                    { 98L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 12L, "Maheshkhali", null, 1, "default_tenant", null, null },
                    { 99L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 12L, "Pekua", null, 1, "default_tenant", null, null },
                    { 100L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 12L, "Ramu", null, 1, "default_tenant", null, null },
                    { 101L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 12L, "Teknaf", null, 1, "default_tenant", null, null },
                    { 102L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 12L, "Ukhia", null, 1, "default_tenant", null, null },
                    { 103L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 13L, "Feni Sadar", null, 1, "default_tenant", null, null },
                    { 104L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 13L, "Chhagalnaiya", null, 1, "default_tenant", null, null },
                    { 105L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 13L, "Daganbhuiyan", null, 1, "default_tenant", null, null },
                    { 106L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 13L, "Parshuram", null, 1, "default_tenant", null, null },
                    { 107L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 13L, "Fulgazi", null, 1, "default_tenant", null, null },
                    { 108L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 13L, "Sonagazi", null, 1, "default_tenant", null, null },
                    { 109L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 14L, "Khagrachhari Sadar", null, 1, "default_tenant", null, null },
                    { 110L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 14L, "Dighinala", null, 1, "default_tenant", null, null },
                    { 111L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 14L, "Lakshmichhari", null, 1, "default_tenant", null, null },
                    { 112L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 14L, "Mahalchhari", null, 1, "default_tenant", null, null },
                    { 113L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 14L, "Manikchhari", null, 1, "default_tenant", null, null },
                    { 114L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 14L, "Matiranga", null, 1, "default_tenant", null, null },
                    { 115L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 14L, "Panchhari", null, 1, "default_tenant", null, null },
                    { 116L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 14L, "Ramgarh", null, 1, "default_tenant", null, null },
                    { 117L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 15L, "Lakshmipur Sadar", null, 1, "default_tenant", null, null },
                    { 118L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 15L, "Kamalnagar", null, 1, "default_tenant", null, null },
                    { 119L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 15L, "Raipur", null, 1, "default_tenant", null, null },
                    { 120L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 15L, "Ramganj", null, 1, "default_tenant", null, null },
                    { 121L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 15L, "Ramgati", null, 1, "default_tenant", null, null },
                    { 122L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 16L, "Noakhali Sadar", null, 1, "default_tenant", null, null },
                    { 123L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 16L, "Begumganj", null, 1, "default_tenant", null, null },
                    { 124L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 16L, "Chatkhil", null, 1, "default_tenant", null, null },
                    { 125L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 16L, "Companiganj", null, 1, "default_tenant", null, null },
                    { 126L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 16L, "Hatiya", null, 1, "default_tenant", null, null },
                    { 127L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 16L, "Kabirhat", null, 1, "default_tenant", null, null },
                    { 128L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 16L, "Senbagh", null, 1, "default_tenant", null, null },
                    { 129L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 16L, "Sonaimuri", null, 1, "default_tenant", null, null },
                    { 130L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 16L, "Subarnachar", null, 1, "default_tenant", null, null },
                    { 131L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 17L, "Rangamati Sadar", null, 1, "default_tenant", null, null },
                    { 132L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 17L, "Bagaichhari", null, 1, "default_tenant", null, null },
                    { 133L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 17L, "Barkal", null, 1, "default_tenant", null, null },
                    { 134L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 17L, "Belaichhari", null, 1, "default_tenant", null, null },
                    { 135L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 17L, "Juraichhari", null, 1, "default_tenant", null, null },
                    { 136L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 17L, "Kaptai", null, 1, "default_tenant", null, null },
                    { 137L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 17L, "Kawkhali", null, 1, "default_tenant", null, null },
                    { 138L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 17L, "Langadu", null, 1, "default_tenant", null, null },
                    { 139L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 17L, "Naniarchar", null, 1, "default_tenant", null, null },
                    { 140L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 17L, "Rajasthali", null, 1, "default_tenant", null, null },
                    { 141L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Dhamrai", null, 1, "default_tenant", null, null },
                    { 142L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Savar", null, 1, "default_tenant", null, null },
                    { 143L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Keraniganj", null, 1, "default_tenant", null, null },
                    { 144L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Nawabganj", null, 1, "default_tenant", null, null },
                    { 145L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Dohar", null, 1, "default_tenant", null, null },
                    { 146L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 19L, "Faridpur Sadar", null, 1, "default_tenant", null, null },
                    { 147L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 19L, "Alfadanga", null, 1, "default_tenant", null, null },
                    { 148L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 19L, "Bhanga", null, 1, "default_tenant", null, null },
                    { 149L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 19L, "Boalmari", null, 1, "default_tenant", null, null },
                    { 150L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 19L, "Charbhadrasan", null, 1, "default_tenant", null, null },
                    { 151L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 19L, "Madhukhali", null, 1, "default_tenant", null, null },
                    { 152L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 19L, "Nagarkanda", null, 1, "default_tenant", null, null },
                    { 153L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 19L, "Sadarpur", null, 1, "default_tenant", null, null },
                    { 154L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 19L, "Saltha", null, 1, "default_tenant", null, null },
                    { 155L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 20L, "Gazipur Sadar", null, 1, "default_tenant", null, null },
                    { 156L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 20L, "Kaliakair", null, 1, "default_tenant", null, null },
                    { 157L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 20L, "Kaliganj", null, 1, "default_tenant", null, null },
                    { 158L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 20L, "Kapasia", null, 1, "default_tenant", null, null },
                    { 159L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 20L, "Sreepur", null, 1, "default_tenant", null, null },
                    { 160L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 21L, "Gopalganj Sadar", null, 1, "default_tenant", null, null },
                    { 161L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 21L, "Kashiani", null, 1, "default_tenant", null, null },
                    { 162L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 21L, "Kotalipara", null, 1, "default_tenant", null, null },
                    { 163L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 21L, "Muksudpur", null, 1, "default_tenant", null, null },
                    { 164L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 21L, "Tungipara", null, 1, "default_tenant", null, null },
                    { 165L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Kishoreganj Sadar", null, 1, "default_tenant", null, null },
                    { 166L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Austagram", null, 1, "default_tenant", null, null },
                    { 167L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Bajitpur", null, 1, "default_tenant", null, null },
                    { 168L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Bhairab", null, 1, "default_tenant", null, null },
                    { 169L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Hossainpur", null, 1, "default_tenant", null, null },
                    { 170L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Itna", null, 1, "default_tenant", null, null },
                    { 171L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Karimganj", null, 1, "default_tenant", null, null },
                    { 172L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Katiadi", null, 1, "default_tenant", null, null },
                    { 173L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Kuliarchar", null, 1, "default_tenant", null, null },
                    { 174L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Mithamain", null, 1, "default_tenant", null, null },
                    { 175L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Nikli", null, 1, "default_tenant", null, null },
                    { 176L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Pakundia", null, 1, "default_tenant", null, null },
                    { 177L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 22L, "Tarail", null, 1, "default_tenant", null, null },
                    { 178L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 23L, "Madaripur Sadar", null, 1, "default_tenant", null, null },
                    { 179L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 23L, "Kalkini", null, 1, "default_tenant", null, null },
                    { 180L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 23L, "Rajoir", null, 1, "default_tenant", null, null },
                    { 181L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 23L, "Shibchar", null, 1, "default_tenant", null, null },
                    { 182L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 24L, "Manikganj Sadar", null, 1, "default_tenant", null, null },
                    { 183L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 24L, "Daulatpur", null, 1, "default_tenant", null, null },
                    { 184L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 24L, "Ghior", null, 1, "default_tenant", null, null },
                    { 185L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 24L, "Harirampur", null, 1, "default_tenant", null, null },
                    { 186L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 24L, "Saturia", null, 1, "default_tenant", null, null },
                    { 187L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 24L, "Shibalaya", null, 1, "default_tenant", null, null },
                    { 188L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 24L, "Singair", null, 1, "default_tenant", null, null },
                    { 189L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 25L, "Munshiganj Sadar", null, 1, "default_tenant", null, null },
                    { 190L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 25L, "Gazaria", null, 1, "default_tenant", null, null },
                    { 191L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 25L, "Lohajang", null, 1, "default_tenant", null, null },
                    { 192L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 25L, "Sirajdikhan", null, 1, "default_tenant", null, null },
                    { 193L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 25L, "Sreenagar", null, 1, "default_tenant", null, null },
                    { 194L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 25L, "Tongibari", null, 1, "default_tenant", null, null },
                    { 195L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 26L, "Narayanganj Sadar", null, 1, "default_tenant", null, null },
                    { 196L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 26L, "Araihazar", null, 1, "default_tenant", null, null },
                    { 197L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 26L, "Bandar", null, 1, "default_tenant", null, null },
                    { 198L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 26L, "Rupganj", null, 1, "default_tenant", null, null },
                    { 199L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 26L, "Sonargaon", null, 1, "default_tenant", null, null },
                    { 200L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 27L, "Narsingdi Sadar", null, 1, "default_tenant", null, null },
                    { 201L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 27L, "Belabo", null, 1, "default_tenant", null, null },
                    { 202L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 27L, "Monohardi", null, 1, "default_tenant", null, null },
                    { 203L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 27L, "Palash", null, 1, "default_tenant", null, null },
                    { 204L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 27L, "Raipura", null, 1, "default_tenant", null, null },
                    { 205L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 27L, "Shibpur", null, 1, "default_tenant", null, null },
                    { 206L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 28L, "Rajbari Sadar", null, 1, "default_tenant", null, null },
                    { 207L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 28L, "Baliakandi", null, 1, "default_tenant", null, null },
                    { 208L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 28L, "Goalandaghat", null, 1, "default_tenant", null, null },
                    { 209L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 28L, "Pangsha", null, 1, "default_tenant", null, null },
                    { 210L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 28L, "Kalukhali", null, 1, "default_tenant", null, null },
                    { 211L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 29L, "Shariatpur Sadar", null, 1, "default_tenant", null, null },
                    { 212L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 29L, "Bhedarganj", null, 1, "default_tenant", null, null },
                    { 213L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 29L, "Damudya", null, 1, "default_tenant", null, null },
                    { 214L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 29L, "Gosairhat", null, 1, "default_tenant", null, null },
                    { 215L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 29L, "Naria", null, 1, "default_tenant", null, null },
                    { 216L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 29L, "Zajira", null, 1, "default_tenant", null, null },
                    { 217L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 30L, "Tangail Sadar", null, 1, "default_tenant", null, null },
                    { 218L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 30L, "Basail", null, 1, "default_tenant", null, null },
                    { 219L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 30L, "Bhuapur", null, 1, "default_tenant", null, null },
                    { 220L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 30L, "Delduar", null, 1, "default_tenant", null, null },
                    { 221L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 30L, "Dhanbari", null, 1, "default_tenant", null, null },
                    { 222L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 30L, "Ghatail", null, 1, "default_tenant", null, null },
                    { 223L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 30L, "Gopalpur", null, 1, "default_tenant", null, null },
                    { 224L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 30L, "Kalihati", null, 1, "default_tenant", null, null },
                    { 225L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 30L, "Madhupur", null, 1, "default_tenant", null, null },
                    { 226L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 30L, "Mirzapur", null, 1, "default_tenant", null, null },
                    { 227L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 30L, "Nagarpur", null, 1, "default_tenant", null, null },
                    { 228L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 30L, "Sakhipur", null, 1, "default_tenant", null, null },
                    { 229L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 31L, "Bagerhat Sadar", null, 1, "default_tenant", null, null },
                    { 230L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 31L, "Chitalmari", null, 1, "default_tenant", null, null },
                    { 231L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 31L, "Fakirhat", null, 1, "default_tenant", null, null },
                    { 232L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 31L, "Kachua", null, 1, "default_tenant", null, null },
                    { 233L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 31L, "Mollahat", null, 1, "default_tenant", null, null },
                    { 234L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 31L, "Mongla", null, 1, "default_tenant", null, null },
                    { 235L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 31L, "Morrelganj", null, 1, "default_tenant", null, null },
                    { 236L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 31L, "Rampal", null, 1, "default_tenant", null, null },
                    { 237L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 31L, "Sarankhola", null, 1, "default_tenant", null, null },
                    { 238L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 32L, "Chuadanga Sadar", null, 1, "default_tenant", null, null },
                    { 239L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 32L, "Alamdanga", null, 1, "default_tenant", null, null },
                    { 240L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 32L, "Damurhuda", null, 1, "default_tenant", null, null },
                    { 241L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 32L, "Jibannagar", null, 1, "default_tenant", null, null },
                    { 242L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 33L, "Jashore Sadar", null, 1, "default_tenant", null, null },
                    { 243L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 33L, "Abhaynagar", null, 1, "default_tenant", null, null },
                    { 244L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 33L, "Bagherpara", null, 1, "default_tenant", null, null },
                    { 245L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 33L, "Chaugachha", null, 1, "default_tenant", null, null },
                    { 246L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 33L, "Jhikargachha", null, 1, "default_tenant", null, null },
                    { 247L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 33L, "Keshabpur", null, 1, "default_tenant", null, null },
                    { 248L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 33L, "Manirampur", null, 1, "default_tenant", null, null },
                    { 249L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 33L, "Sharsha", null, 1, "default_tenant", null, null },
                    { 250L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 34L, "Jhenaidah Sadar", null, 1, "default_tenant", null, null },
                    { 251L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 34L, "Harinakunda", null, 1, "default_tenant", null, null },
                    { 252L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 34L, "Kaliganj", null, 1, "default_tenant", null, null },
                    { 253L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 34L, "Kotchandpur", null, 1, "default_tenant", null, null },
                    { 254L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 34L, "Maheshpur", null, 1, "default_tenant", null, null },
                    { 255L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 34L, "Shailkupa", null, 1, "default_tenant", null, null },
                    { 256L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Batiaghata", null, 1, "default_tenant", null, null },
                    { 257L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Dacope", null, 1, "default_tenant", null, null },
                    { 258L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Dumuria", null, 1, "default_tenant", null, null },
                    { 259L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Dighalia", null, 1, "default_tenant", null, null },
                    { 260L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Koyra", null, 1, "default_tenant", null, null },
                    { 261L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Paikgachha", null, 1, "default_tenant", null, null },
                    { 262L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Phultala", null, 1, "default_tenant", null, null },
                    { 263L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Rupsa", null, 1, "default_tenant", null, null },
                    { 264L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Terokhada", null, 1, "default_tenant", null, null },
                    { 265L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 36L, "Kushtia Sadar", null, 1, "default_tenant", null, null },
                    { 266L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 36L, "Bheramara", null, 1, "default_tenant", null, null },
                    { 267L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 36L, "Daulatpur", null, 1, "default_tenant", null, null },
                    { 268L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 36L, "Khoksa", null, 1, "default_tenant", null, null },
                    { 269L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 36L, "Kumarkhali", null, 1, "default_tenant", null, null },
                    { 270L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 36L, "Mirpur", null, 1, "default_tenant", null, null },
                    { 271L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 37L, "Magura Sadar", null, 1, "default_tenant", null, null },
                    { 272L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 37L, "Mohammadpur", null, 1, "default_tenant", null, null },
                    { 273L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 37L, "Shalikha", null, 1, "default_tenant", null, null },
                    { 274L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 37L, "Sreepur", null, 1, "default_tenant", null, null },
                    { 275L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 38L, "Meherpur Sadar", null, 1, "default_tenant", null, null },
                    { 276L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 38L, "Gangni", null, 1, "default_tenant", null, null },
                    { 277L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 38L, "Mujibnagar", null, 1, "default_tenant", null, null },
                    { 278L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 39L, "Narail Sadar", null, 1, "default_tenant", null, null },
                    { 279L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 39L, "Kalia", null, 1, "default_tenant", null, null },
                    { 280L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 39L, "Lohagara", null, 1, "default_tenant", null, null },
                    { 281L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 40L, "Satkhira Sadar", null, 1, "default_tenant", null, null },
                    { 282L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 40L, "Assasuni", null, 1, "default_tenant", null, null },
                    { 283L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 40L, "Debhata", null, 1, "default_tenant", null, null },
                    { 284L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 40L, "Kalaroa", null, 1, "default_tenant", null, null },
                    { 285L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 40L, "Kaliganj", null, 1, "default_tenant", null, null },
                    { 286L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 40L, "Shyamnagar", null, 1, "default_tenant", null, null },
                    { 287L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 40L, "Tala", null, 1, "default_tenant", null, null },
                    { 288L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 41L, "Jamalpur Sadar", null, 1, "default_tenant", null, null },
                    { 289L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 41L, "Bakshiganj", null, 1, "default_tenant", null, null },
                    { 290L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 41L, "Dewanganj", null, 1, "default_tenant", null, null },
                    { 291L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 41L, "Islampur", null, 1, "default_tenant", null, null },
                    { 292L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 41L, "Madarganj", null, 1, "default_tenant", null, null },
                    { 293L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 41L, "Melandaha", null, 1, "default_tenant", null, null },
                    { 294L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 41L, "Sarishabari", null, 1, "default_tenant", null, null },
                    { 295L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 42L, "Mymensingh Sadar", null, 1, "default_tenant", null, null },
                    { 296L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 42L, "Bhaluka", null, 1, "default_tenant", null, null },
                    { 297L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 42L, "Dhobaura", null, 1, "default_tenant", null, null },
                    { 298L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 42L, "Fulbaria", null, 1, "default_tenant", null, null },
                    { 299L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 42L, "Gaffargaon", null, 1, "default_tenant", null, null },
                    { 300L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 42L, "Gauripur", null, 1, "default_tenant", null, null },
                    { 301L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 42L, "Haluaghat", null, 1, "default_tenant", null, null },
                    { 302L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 42L, "Ishwarganj", null, 1, "default_tenant", null, null },
                    { 303L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 42L, "Muktagachha", null, 1, "default_tenant", null, null },
                    { 304L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 42L, "Nandail", null, 1, "default_tenant", null, null },
                    { 305L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 42L, "Phulpur", null, 1, "default_tenant", null, null },
                    { 306L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 42L, "Trishal", null, 1, "default_tenant", null, null },
                    { 307L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 43L, "Netrokona Sadar", null, 1, "default_tenant", null, null },
                    { 308L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 43L, "Atpara", null, 1, "default_tenant", null, null },
                    { 309L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 43L, "Barhatta", null, 1, "default_tenant", null, null },
                    { 310L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 43L, "Durgapur", null, 1, "default_tenant", null, null },
                    { 311L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 43L, "Kalmakanda", null, 1, "default_tenant", null, null },
                    { 312L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 43L, "Kendua", null, 1, "default_tenant", null, null },
                    { 313L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 43L, "Khaliajuri", null, 1, "default_tenant", null, null },
                    { 314L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 43L, "Madan", null, 1, "default_tenant", null, null },
                    { 315L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 43L, "Mohanganj", null, 1, "default_tenant", null, null },
                    { 316L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 43L, "Purbadhala", null, 1, "default_tenant", null, null },
                    { 317L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 44L, "Sherpur Sadar", null, 1, "default_tenant", null, null },
                    { 318L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 44L, "Jhenaigati", null, 1, "default_tenant", null, null },
                    { 319L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 44L, "Nakla", null, 1, "default_tenant", null, null },
                    { 320L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 44L, "Nalitabari", null, 1, "default_tenant", null, null },
                    { 321L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 44L, "Sreebardi", null, 1, "default_tenant", null, null },
                    { 322L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 45L, "Bogura Sadar", null, 1, "default_tenant", null, null },
                    { 323L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 45L, "Adamdighi", null, 1, "default_tenant", null, null },
                    { 324L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 45L, "Dhunat", null, 1, "default_tenant", null, null },
                    { 325L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 45L, "Dhupchanchia", null, 1, "default_tenant", null, null },
                    { 326L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 45L, "Gabtali", null, 1, "default_tenant", null, null },
                    { 327L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 45L, "Kahaloo", null, 1, "default_tenant", null, null },
                    { 328L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 45L, "Nandigram", null, 1, "default_tenant", null, null },
                    { 329L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 45L, "Sariakandi", null, 1, "default_tenant", null, null },
                    { 330L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 45L, "Shajahanpur", null, 1, "default_tenant", null, null },
                    { 331L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 45L, "Sherpur", null, 1, "default_tenant", null, null },
                    { 332L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 45L, "Shibganj", null, 1, "default_tenant", null, null },
                    { 333L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 45L, "Sonatala", null, 1, "default_tenant", null, null },
                    { 334L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 46L, "Chapainawabganj Sadar", null, 1, "default_tenant", null, null },
                    { 335L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 46L, "Bholahat", null, 1, "default_tenant", null, null },
                    { 336L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 46L, "Gomastapur", null, 1, "default_tenant", null, null },
                    { 337L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 46L, "Nachole", null, 1, "default_tenant", null, null },
                    { 338L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 46L, "Shibganj", null, 1, "default_tenant", null, null },
                    { 339L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 47L, "Joypurhat Sadar", null, 1, "default_tenant", null, null },
                    { 340L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 47L, "Akkelpur", null, 1, "default_tenant", null, null },
                    { 341L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 47L, "Kalai", null, 1, "default_tenant", null, null },
                    { 342L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 47L, "Khetlal", null, 1, "default_tenant", null, null },
                    { 343L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 47L, "Panchbibi", null, 1, "default_tenant", null, null },
                    { 344L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 48L, "Naogaon Sadar", null, 1, "default_tenant", null, null },
                    { 345L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 48L, "Atrai", null, 1, "default_tenant", null, null },
                    { 346L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 48L, "Badalgachhi", null, 1, "default_tenant", null, null },
                    { 347L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 48L, "Dhamoirhat", null, 1, "default_tenant", null, null },
                    { 348L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 48L, "Manda", null, 1, "default_tenant", null, null },
                    { 349L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 48L, "Mahadebpur", null, 1, "default_tenant", null, null },
                    { 350L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 48L, "Niamatpur", null, 1, "default_tenant", null, null },
                    { 351L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 48L, "Patnitala", null, 1, "default_tenant", null, null },
                    { 352L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 48L, "Porsha", null, 1, "default_tenant", null, null },
                    { 353L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 48L, "Raninagar", null, 1, "default_tenant", null, null },
                    { 354L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 48L, "Sapahar", null, 1, "default_tenant", null, null },
                    { 355L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 49L, "Natore Sadar", null, 1, "default_tenant", null, null },
                    { 356L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 49L, "Bagatipara", null, 1, "default_tenant", null, null },
                    { 357L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 49L, "Baraigram", null, 1, "default_tenant", null, null },
                    { 358L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 49L, "Gurudaspur", null, 1, "default_tenant", null, null },
                    { 359L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 49L, "Lalpur", null, 1, "default_tenant", null, null },
                    { 360L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 49L, "Singra", null, 1, "default_tenant", null, null },
                    { 361L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 50L, "Pabna Sadar", null, 1, "default_tenant", null, null },
                    { 362L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 50L, "Atgharia", null, 1, "default_tenant", null, null },
                    { 363L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 50L, "Bera", null, 1, "default_tenant", null, null },
                    { 364L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 50L, "Bhangura", null, 1, "default_tenant", null, null },
                    { 365L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 50L, "Chatmohar", null, 1, "default_tenant", null, null },
                    { 366L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 50L, "Faridpur", null, 1, "default_tenant", null, null },
                    { 367L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 50L, "Ishwardi", null, 1, "default_tenant", null, null },
                    { 368L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 50L, "Santhia", null, 1, "default_tenant", null, null },
                    { 369L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 50L, "Sujanagar", null, 1, "default_tenant", null, null },
                    { 370L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Bagha", null, 1, "default_tenant", null, null },
                    { 371L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Bagmara", null, 1, "default_tenant", null, null },
                    { 372L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Charghat", null, 1, "default_tenant", null, null },
                    { 373L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Durgapur", null, 1, "default_tenant", null, null },
                    { 374L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Godagari", null, 1, "default_tenant", null, null },
                    { 375L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Mohonpur", null, 1, "default_tenant", null, null },
                    { 376L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Paba", null, 1, "default_tenant", null, null },
                    { 377L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Puthia", null, 1, "default_tenant", null, null },
                    { 378L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Tanore", null, 1, "default_tenant", null, null },
                    { 379L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 52L, "Sirajganj Sadar", null, 1, "default_tenant", null, null },
                    { 380L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 52L, "Belkuchi", null, 1, "default_tenant", null, null },
                    { 381L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 52L, "Chauhali", null, 1, "default_tenant", null, null },
                    { 382L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 52L, "Kamarkhanda", null, 1, "default_tenant", null, null },
                    { 383L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 52L, "Kazipur", null, 1, "default_tenant", null, null },
                    { 384L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 52L, "Raiganj", null, 1, "default_tenant", null, null },
                    { 385L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 52L, "Shahjadpur", null, 1, "default_tenant", null, null },
                    { 386L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 52L, "Tarash", null, 1, "default_tenant", null, null },
                    { 387L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 52L, "Ullapara", null, 1, "default_tenant", null, null },
                    { 388L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Dinajpur Sadar", null, 1, "default_tenant", null, null },
                    { 389L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Birampur", null, 1, "default_tenant", null, null },
                    { 390L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Birganj", null, 1, "default_tenant", null, null },
                    { 391L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Biral", null, 1, "default_tenant", null, null },
                    { 392L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Bochaganj", null, 1, "default_tenant", null, null },
                    { 393L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Chirirbandar", null, 1, "default_tenant", null, null },
                    { 394L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Fulbari", null, 1, "default_tenant", null, null },
                    { 395L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Ghoraghat", null, 1, "default_tenant", null, null },
                    { 396L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Hakimpur", null, 1, "default_tenant", null, null },
                    { 397L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Kaharole", null, 1, "default_tenant", null, null },
                    { 398L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Khansama", null, 1, "default_tenant", null, null },
                    { 399L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Nawabganj", null, 1, "default_tenant", null, null },
                    { 400L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 53L, "Parbatipur", null, 1, "default_tenant", null, null },
                    { 401L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 54L, "Gaibandha Sadar", null, 1, "default_tenant", null, null },
                    { 402L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 54L, "Fulchhari", null, 1, "default_tenant", null, null },
                    { 403L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 54L, "Gobindaganj", null, 1, "default_tenant", null, null },
                    { 404L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 54L, "Palashbari", null, 1, "default_tenant", null, null },
                    { 405L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 54L, "Sadullapur", null, 1, "default_tenant", null, null },
                    { 406L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 54L, "Saghata", null, 1, "default_tenant", null, null },
                    { 407L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 54L, "Sundarganj", null, 1, "default_tenant", null, null },
                    { 408L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 55L, "Kurigram Sadar", null, 1, "default_tenant", null, null },
                    { 409L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 55L, "Bhurungamari", null, 1, "default_tenant", null, null },
                    { 410L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 55L, "Char Rajibpur", null, 1, "default_tenant", null, null },
                    { 411L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 55L, "Chilmari", null, 1, "default_tenant", null, null },
                    { 412L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 55L, "Nageshwari", null, 1, "default_tenant", null, null },
                    { 413L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 55L, "Phulbari", null, 1, "default_tenant", null, null },
                    { 414L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 55L, "Rajarhat", null, 1, "default_tenant", null, null },
                    { 415L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 55L, "Raomari", null, 1, "default_tenant", null, null },
                    { 416L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 55L, "Ulipur", null, 1, "default_tenant", null, null },
                    { 417L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 56L, "Lalmonirhat Sadar", null, 1, "default_tenant", null, null },
                    { 418L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 56L, "Aditmari", null, 1, "default_tenant", null, null },
                    { 419L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 56L, "Hatibandha", null, 1, "default_tenant", null, null },
                    { 420L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 56L, "Kaliganj", null, 1, "default_tenant", null, null },
                    { 421L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 56L, "Patgram", null, 1, "default_tenant", null, null },
                    { 422L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 57L, "Nilphamari Sadar", null, 1, "default_tenant", null, null },
                    { 423L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 57L, "Dimla", null, 1, "default_tenant", null, null },
                    { 424L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 57L, "Domar", null, 1, "default_tenant", null, null },
                    { 425L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 57L, "Jaldhaka", null, 1, "default_tenant", null, null },
                    { 426L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 57L, "Kishoreganj", null, 1, "default_tenant", null, null },
                    { 427L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 57L, "Saidpur", null, 1, "default_tenant", null, null },
                    { 428L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 58L, "Panchagarh Sadar", null, 1, "default_tenant", null, null },
                    { 429L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 58L, "Atwari", null, 1, "default_tenant", null, null },
                    { 430L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 58L, "Boda", null, 1, "default_tenant", null, null },
                    { 431L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 58L, "Debiganj", null, 1, "default_tenant", null, null },
                    { 432L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 58L, "Tetulia", null, 1, "default_tenant", null, null },
                    { 433L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 59L, "Rangpur Sadar", null, 1, "default_tenant", null, null },
                    { 434L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 59L, "Badarganj", null, 1, "default_tenant", null, null },
                    { 435L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 59L, "Gangachara", null, 1, "default_tenant", null, null },
                    { 436L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 59L, "Kaunia", null, 1, "default_tenant", null, null },
                    { 437L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 59L, "Mithapukur", null, 1, "default_tenant", null, null },
                    { 438L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 59L, "Pirgachha", null, 1, "default_tenant", null, null },
                    { 439L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 59L, "Pirganj", null, 1, "default_tenant", null, null },
                    { 440L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 59L, "Taraganj", null, 1, "default_tenant", null, null },
                    { 441L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 60L, "Thakurgaon Sadar", null, 1, "default_tenant", null, null },
                    { 442L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 60L, "Baliadangi", null, 1, "default_tenant", null, null },
                    { 443L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 60L, "Haripur", null, 1, "default_tenant", null, null },
                    { 444L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 60L, "Pirganj", null, 1, "default_tenant", null, null },
                    { 445L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 60L, "Ranisankail", null, 1, "default_tenant", null, null },
                    { 446L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 61L, "Habiganj Sadar", null, 1, "default_tenant", null, null },
                    { 447L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 61L, "Ajmiriganj", null, 1, "default_tenant", null, null },
                    { 448L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 61L, "Bahubal", null, 1, "default_tenant", null, null },
                    { 449L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 61L, "Baniyachong", null, 1, "default_tenant", null, null },
                    { 450L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 61L, "Chunarughat", null, 1, "default_tenant", null, null },
                    { 451L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 61L, "Lakhai", null, 1, "default_tenant", null, null },
                    { 452L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 61L, "Madhabpur", null, 1, "default_tenant", null, null },
                    { 453L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 61L, "Nabiganj", null, 1, "default_tenant", null, null },
                    { 454L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 61L, "Sayestaganj", null, 1, "default_tenant", null, null },
                    { 455L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 62L, "Moulvibazar Sadar", null, 1, "default_tenant", null, null },
                    { 456L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 62L, "Barlekha", null, 1, "default_tenant", null, null },
                    { 457L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 62L, "Juri", null, 1, "default_tenant", null, null },
                    { 458L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 62L, "Kamalganj", null, 1, "default_tenant", null, null },
                    { 459L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 62L, "Kulaura", null, 1, "default_tenant", null, null },
                    { 460L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 62L, "Rajnagar", null, 1, "default_tenant", null, null },
                    { 461L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 62L, "Sreemangal", null, 1, "default_tenant", null, null },
                    { 462L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 63L, "Sunamganj Sadar", null, 1, "default_tenant", null, null },
                    { 463L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 63L, "Bishwamvarpur", null, 1, "default_tenant", null, null },
                    { 464L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 63L, "Chhatak", null, 1, "default_tenant", null, null },
                    { 465L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 63L, "Derai", null, 1, "default_tenant", null, null },
                    { 466L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 63L, "Dharampasha", null, 1, "default_tenant", null, null },
                    { 467L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 63L, "Dowarabazar", null, 1, "default_tenant", null, null },
                    { 468L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 63L, "Jagannathpur", null, 1, "default_tenant", null, null },
                    { 469L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 63L, "Jamalganj", null, 1, "default_tenant", null, null },
                    { 470L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 63L, "Sullah", null, 1, "default_tenant", null, null },
                    { 471L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 63L, "Tahirpur", null, 1, "default_tenant", null, null },
                    { 472L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 63L, "South Sunamganj", null, 1, "default_tenant", null, null },
                    { 473L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Sylhet Sadar", null, 1, "default_tenant", null, null },
                    { 474L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Balaganj", null, 1, "default_tenant", null, null },
                    { 475L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Beanibazar", null, 1, "default_tenant", null, null },
                    { 476L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Bishwanath", null, 1, "default_tenant", null, null },
                    { 477L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Companiganj", null, 1, "default_tenant", null, null },
                    { 478L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Fenchuganj", null, 1, "default_tenant", null, null },
                    { 479L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Golapganj", null, 1, "default_tenant", null, null },
                    { 480L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Gowainghat", null, 1, "default_tenant", null, null },
                    { 481L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Jaintiapur", null, 1, "default_tenant", null, null },
                    { 482L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Kanaighat", null, 1, "default_tenant", null, null },
                    { 483L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Osmani Nagar", null, 1, "default_tenant", null, null },
                    { 484L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Zakiganj", null, 1, "default_tenant", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_HomeDistrictId",
                table: "CandidateProfiles",
                column: "HomeDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_HomeDivisionId",
                table: "CandidateProfiles",
                column: "HomeDivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_HomeThanaId",
                table: "CandidateProfiles",
                column: "HomeThanaId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_PresentDistrictId",
                table: "CandidateProfiles",
                column: "PresentDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_PresentDivisionId",
                table: "CandidateProfiles",
                column: "PresentDivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_PresentThanaId",
                table: "CandidateProfiles",
                column: "PresentThanaId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_DivisionId",
                table: "Districts",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_Name",
                table: "Divisions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Thanas_DistrictId",
                table: "Thanas",
                column: "DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_Districts_HomeDistrictId",
                table: "CandidateProfiles",
                column: "HomeDistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_Districts_PresentDistrictId",
                table: "CandidateProfiles",
                column: "PresentDistrictId",
                principalTable: "Districts",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_Divisions_HomeDivisionId",
                table: "CandidateProfiles",
                column: "HomeDivisionId",
                principalTable: "Divisions",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_Divisions_PresentDivisionId",
                table: "CandidateProfiles",
                column: "PresentDivisionId",
                principalTable: "Divisions",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_Thanas_HomeThanaId",
                table: "CandidateProfiles",
                column: "HomeThanaId",
                principalTable: "Thanas",
                principalColumn: "ThanaId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_Thanas_PresentThanaId",
                table: "CandidateProfiles",
                column: "PresentThanaId",
                principalTable: "Thanas",
                principalColumn: "ThanaId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Districts_HomeDistrictId",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Districts_PresentDistrictId",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Divisions_HomeDivisionId",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Divisions_PresentDivisionId",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Thanas_HomeThanaId",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Thanas_PresentThanaId",
                table: "CandidateProfiles");

            migrationBuilder.DropTable(
                name: "Thanas");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_HomeDistrictId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_HomeDivisionId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_HomeThanaId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_PresentDistrictId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_PresentDivisionId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_PresentThanaId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "HomeDistrictId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "HomeDivisionId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "HomeThanaId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "MobileOperator",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "PresentDistrictId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "PresentDivisionId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "PresentThanaId",
                table: "CandidateProfiles");

            migrationBuilder.RenameColumn(
                name: "PresentAddressDetail",
                table: "CandidateProfiles",
                newName: "PresentAddress");

            migrationBuilder.RenameColumn(
                name: "PermanentAddressDetail",
                table: "CandidateProfiles",
                newName: "PermanentAddress");
        }
    }
}
