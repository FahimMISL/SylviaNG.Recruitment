using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class SkillLibraryItemConfiguration : IEntityTypeConfiguration<SkillLibraryItem>
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<SkillLibraryItem> builder)
        {
            builder.ToTable("SkillLibraryItems");
            builder.HasKey(s => s.SkillLibraryItemId);

            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(s => s.Name).IsUnique();

            var names = new[]
            {
                "C#", ".NET", "Java", "JavaScript", "TypeScript", "Angular", "React", "SQL", "PostgreSQL",
                "Microsoft Excel", "Data Analysis", "Python", "AWS", "Docker", "Git", "Agile/Scrum",
                "Project Management", "Leadership", "Communication", "Customer Service"
            };

            builder.HasData(names.Select((name, index) => new
            {
                SkillLibraryItemId = (long)(index + 1),
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
