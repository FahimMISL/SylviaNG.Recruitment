using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class RemoveJobPostingSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "JobPostings",
                keyColumn: "JobPostingId",
                keyValue: 2L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "JobPostings",
                columns: new[] { "JobPostingId", "ApplicationFeeAmount", "ApplicationFeeCurrency", "CircularType", "ClosingDate", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DepartmentId", "Description", "EmploymentType", "HiringPipelineId", "IsActive", "JobPostingCode", "Location", "MaxAge", "MaxSalary", "MinAge", "MinEducationLevel", "MinExperienceYears", "MinSalary", "NumberOfPositions", "PostingDate", "Remarks", "RequiredDistrict", "Requirements", "SalaryCurrency", "Status", "TenantId", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1L, null, null, "Both", new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "We are looking for a Senior Software Engineer to join our team.", "FullTime", null, true, "JOB-2026-000001", null, null, 120000m, null, null, null, 80000m, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "5+ years of experience in .NET, C#, and SQL Server.", null, "Open", "default_tenant", "Senior Software Engineer", null, null },
                    { 2L, null, null, "Both", new DateTime(2025, 7, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1L, null, null, 1L, "Looking for a creative UI/UX Designer.", "FullTime", null, true, "JOB-2026-000002", null, null, 80000m, null, null, null, 50000m, 1, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "3+ years of experience in Figma and Adobe XD.", null, "Open", "default_tenant", "UI/UX Designer", null, null }
                });
        }
    }
}
