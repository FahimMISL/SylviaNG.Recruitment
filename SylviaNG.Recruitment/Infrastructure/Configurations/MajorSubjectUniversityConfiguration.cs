using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class MajorSubjectUniversityConfiguration : IEntityTypeConfiguration<MajorSubjectUniversity>
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly string[] Values =
        {
            "Computer Science & Engineering", "Business Administration", "Accounting", "Finance", "Marketing",
            "Management", "Economics", "English", "Law", "Electrical & Electronic Engineering",
            "Civil Engineering", "Mechanical Engineering", "Pharmacy", "Medicine", "Architecture",
            "Mathematics", "Statistics", "Psychology", "Sociology"
        };

        public void Configure(EntityTypeBuilder<MajorSubjectUniversity> builder)
        {
            builder.ToTable("MajorSubjectsUniversity");
            builder.HasKey(m => m.MajorSubjectUniversityId);

            builder.Property(m => m.Name).IsRequired().HasMaxLength(150);
            builder.HasIndex(m => m.Name).IsUnique();

            builder.HasData(Values.Select((name, index) => new
            {
                MajorSubjectUniversityId = (long)(index + 1),
                Name = name,
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
