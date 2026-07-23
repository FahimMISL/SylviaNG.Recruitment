using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class EducationBoardConfiguration : IEntityTypeConfiguration<EducationBoard>
    {
        private static readonly DateTime SeedCreatedAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Matches the company's reference Board list exactly (Dhaka/Rajshahi/... boards +
        // Madrasah + Technical Education + Bangladesh Open University + Edexcel + a catch-all
        // for foreign-educated candidates).
        private static readonly (string Code, string Name)[] Boards =
        {
            ("B001", "Dhaka Board"),
            ("B002", "Chittagong Board"),
            ("B003", "Barisal Board"),
            ("B004", "Jessore Board"),
            ("B005", "Rajshahi Board"),
            ("B006", "Comilla Board"),
            ("B007", "Dinajpur Board"),
            ("B008", "Sylhet Board"),
            ("B009", "Madrasah Board"),
            ("B010", "Technical Education Board"),
            ("B011", "Edexcel"),
            ("B012", "Others for Foreign Country"),
            ("B013", "Bangladesh Open University"),
        };

        public void Configure(EntityTypeBuilder<EducationBoard> builder)
        {
            builder.ToTable("EducationBoards");
            builder.HasKey(b => b.EducationBoardId);

            builder.Property(b => b.Code).IsRequired().HasMaxLength(10);
            builder.Property(b => b.Name).IsRequired().HasMaxLength(150);
            builder.HasIndex(b => b.Name).IsUnique();

            builder.HasData(Boards.Select((b, index) => new
            {
                EducationBoardId = (long)(index + 1),
                b.Code,
                b.Name,
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
