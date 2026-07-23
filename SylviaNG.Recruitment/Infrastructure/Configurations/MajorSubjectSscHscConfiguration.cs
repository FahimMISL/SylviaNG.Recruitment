using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class MajorSubjectSscHscConfiguration : IEntityTypeConfiguration<MajorSubjectSscHsc>
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly string[] Values =
        {
            "Science", "Arts", "Humanities", "Commerce", "Business Studies", "Technical", "Vocational"
        };

        public void Configure(EntityTypeBuilder<MajorSubjectSscHsc> builder)
        {
            builder.ToTable("MajorSubjectsSscHsc");
            builder.HasKey(m => m.MajorSubjectSscHscId);

            builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(m => m.Name).IsUnique();

            builder.HasData(Values.Select((name, index) => new
            {
                MajorSubjectSscHscId = (long)(index + 1),
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
