using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly string[] Values = { "Engineering", "Business & Sales", "Human Resources", "Finance", "Marketing", "Operations", "Customer Support", "Legal & Compliance" };

        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");
            builder.HasKey(d => d.DepartmentId);

            builder.Property(d => d.Name).IsRequired().HasMaxLength(100);
            // Multi-tenant: a company can add a custom department with the same name another
            // company (or the global seed list) already uses - uniqueness only applies within
            // the same CompanyId (including among the null/global rows).
            builder.HasIndex(d => new { d.CompanyId, d.Name }).IsUnique();

            builder.HasData(Values.Select((name, index) => new
            {
                DepartmentId = (long)(index + 1),
                Name = name,
                CompanyId = (long?)null,
                TenantId = "default_tenant",
                Remarks = (string?)null,
                CreatedAt = SeedCreatedAt,
                CreatedBy = 1L,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (long?)null,
                DeletedAt = (DateTime?)null,
                DeletedBy = (long?)null,
                Status = 1
            }));
        }
    }
}
