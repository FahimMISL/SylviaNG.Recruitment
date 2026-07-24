using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class AddMetropolitanThanas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Thanas",
                columns: new[] { "ThanaId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DistrictId", "Name", "Remarks", "Status", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 485L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Adabor", null, 1, "default_tenant", null, null },
                    { 486L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Badda", null, 1, "default_tenant", null, null },
                    { 487L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Bangshal", null, 1, "default_tenant", null, null },
                    { 488L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Bhashantek", null, 1, "default_tenant", null, null },
                    { 489L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Bimanbandar", null, 1, "default_tenant", null, null },
                    { 490L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Cantonment", null, 1, "default_tenant", null, null },
                    { 491L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Chackbazar", null, 1, "default_tenant", null, null },
                    { 492L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Dakshinkhan", null, 1, "default_tenant", null, null },
                    { 493L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Darus Salam", null, 1, "default_tenant", null, null },
                    { 494L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Demra", null, 1, "default_tenant", null, null },
                    { 495L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Dhanmondi", null, 1, "default_tenant", null, null },
                    { 496L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Gandaria", null, 1, "default_tenant", null, null },
                    { 497L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Gulshan", null, 1, "default_tenant", null, null },
                    { 498L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Hatirjheel", null, 1, "default_tenant", null, null },
                    { 499L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Hazaribagh", null, 1, "default_tenant", null, null },
                    { 500L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Jatrabari", null, 1, "default_tenant", null, null },
                    { 501L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Kadamtali", null, 1, "default_tenant", null, null },
                    { 502L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Kafrul", null, 1, "default_tenant", null, null },
                    { 503L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Kalabagan", null, 1, "default_tenant", null, null },
                    { 504L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Kamrangirchar", null, 1, "default_tenant", null, null },
                    { 505L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Khilgaon", null, 1, "default_tenant", null, null },
                    { 506L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Khilkhet", null, 1, "default_tenant", null, null },
                    { 507L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Kotwali", null, 1, "default_tenant", null, null },
                    { 508L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Lalbagh", null, 1, "default_tenant", null, null },
                    { 509L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Mirpur", null, 1, "default_tenant", null, null },
                    { 510L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Mohammadpur", null, 1, "default_tenant", null, null },
                    { 511L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Motijheel", null, 1, "default_tenant", null, null },
                    { 512L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Mugda", null, 1, "default_tenant", null, null },
                    { 513L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "New Market", null, 1, "default_tenant", null, null },
                    { 514L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Pallabi", null, 1, "default_tenant", null, null },
                    { 515L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Paltan", null, 1, "default_tenant", null, null },
                    { 516L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Ramna", null, 1, "default_tenant", null, null },
                    { 517L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Rampura", null, 1, "default_tenant", null, null },
                    { 518L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Rupnagar", null, 1, "default_tenant", null, null },
                    { 519L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Sabujbagh", null, 1, "default_tenant", null, null },
                    { 520L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Shah Ali", null, 1, "default_tenant", null, null },
                    { 521L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Shahbagh", null, 1, "default_tenant", null, null },
                    { 522L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Shahjahanpur", null, 1, "default_tenant", null, null },
                    { 523L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Sher-e-Bangla Nagar", null, 1, "default_tenant", null, null },
                    { 524L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Shyampur", null, 1, "default_tenant", null, null },
                    { 525L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Sutrapur", null, 1, "default_tenant", null, null },
                    { 526L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Tejgaon", null, 1, "default_tenant", null, null },
                    { 527L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Tejgaon Industrial Area", null, 1, "default_tenant", null, null },
                    { 528L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Turag", null, 1, "default_tenant", null, null },
                    { 529L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Uttara East", null, 1, "default_tenant", null, null },
                    { 530L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Uttara West", null, 1, "default_tenant", null, null },
                    { 531L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Uttarkhan", null, 1, "default_tenant", null, null },
                    { 532L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Vatara", null, 1, "default_tenant", null, null },
                    { 533L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 18L, "Wari", null, 1, "default_tenant", null, null },
                    { 534L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Akbar Shah", null, 1, "default_tenant", null, null },
                    { 535L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Bakalia", null, 1, "default_tenant", null, null },
                    { 536L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Bandar", null, 1, "default_tenant", null, null },
                    { 537L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Bayazid Bostami", null, 1, "default_tenant", null, null },
                    { 538L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Chandgaon", null, 1, "default_tenant", null, null },
                    { 539L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Chawkbazar", null, 1, "default_tenant", null, null },
                    { 540L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Double Mooring", null, 1, "default_tenant", null, null },
                    { 541L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "EPZ", null, 1, "default_tenant", null, null },
                    { 542L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Halishahar", null, 1, "default_tenant", null, null },
                    { 543L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Khulshi", null, 1, "default_tenant", null, null },
                    { 544L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Kotwali", null, 1, "default_tenant", null, null },
                    { 545L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Pahartali", null, 1, "default_tenant", null, null },
                    { 546L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Panchlaish", null, 1, "default_tenant", null, null },
                    { 547L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Patenga", null, 1, "default_tenant", null, null },
                    { 548L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 10L, "Sadarghat", null, 1, "default_tenant", null, null },
                    { 549L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Khulna Airport", null, 1, "default_tenant", null, null },
                    { 550L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Khalishpur", null, 1, "default_tenant", null, null },
                    { 551L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Khan Jahan Ali", null, 1, "default_tenant", null, null },
                    { 552L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Kotwali", null, 1, "default_tenant", null, null },
                    { 553L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Sonadanga", null, 1, "default_tenant", null, null },
                    { 554L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Daulatpur", null, 1, "default_tenant", null, null },
                    { 555L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Harintana", null, 1, "default_tenant", null, null },
                    { 556L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 35L, "Lobonchora", null, 1, "default_tenant", null, null },
                    { 557L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Boalia", null, 1, "default_tenant", null, null },
                    { 558L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Rajpara", null, 1, "default_tenant", null, null },
                    { 559L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Shah Makhdum", null, 1, "default_tenant", null, null },
                    { 560L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Matihar", null, 1, "default_tenant", null, null },
                    { 561L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Chandrima", null, 1, "default_tenant", null, null },
                    { 562L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Kashiadanga", null, 1, "default_tenant", null, null },
                    { 563L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Katakhali", null, 1, "default_tenant", null, null },
                    { 564L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 51L, "Rajshahi Airport", null, 1, "default_tenant", null, null },
                    { 565L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Kotwali", null, 1, "default_tenant", null, null },
                    { 566L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Jalalabad", null, 1, "default_tenant", null, null },
                    { 567L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Sylhet Airport", null, 1, "default_tenant", null, null },
                    { 568L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "South Surma", null, 1, "default_tenant", null, null },
                    { 569L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Shah Poran", null, 1, "default_tenant", null, null },
                    { 570L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 64L, "Moglabazar", null, 1, "default_tenant", null, null },
                    { 571L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Kotwali", null, 1, "default_tenant", null, null },
                    { 572L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Barisal Airport", null, 1, "default_tenant", null, null },
                    { 573L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Bandar", null, 1, "default_tenant", null, null },
                    { 574L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Kaunia", null, 1, "default_tenant", null, null },
                    { 575L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 59L, "Kotwali", null, 1, "default_tenant", null, null },
                    { 576L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 59L, "Mahiganj", null, 1, "default_tenant", null, null },
                    { 577L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 59L, "Tajhat", null, 1, "default_tenant", null, null },
                    { 578L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 59L, "Haragach", null, 1, "default_tenant", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 485L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 486L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 487L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 488L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 489L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 490L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 491L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 492L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 493L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 494L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 495L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 496L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 497L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 498L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 499L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 500L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 501L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 502L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 503L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 504L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 505L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 506L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 507L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 508L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 509L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 510L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 511L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 512L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 513L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 514L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 515L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 516L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 517L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 518L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 519L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 520L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 521L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 522L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 523L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 524L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 525L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 526L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 527L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 528L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 529L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 530L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 531L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 532L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 533L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 534L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 535L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 536L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 537L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 538L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 539L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 540L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 541L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 542L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 543L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 544L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 545L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 546L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 547L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 548L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 549L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 550L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 551L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 552L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 553L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 554L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 555L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 556L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 557L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 558L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 559L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 560L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 561L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 562L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 563L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 564L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 565L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 566L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 567L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 568L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 569L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 570L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 571L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 572L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 573L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 574L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 575L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 576L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 577L);

            migrationBuilder.DeleteData(
                table: "Thanas",
                keyColumn: "ThanaId",
                keyValue: 578L);
        }
    }
}
