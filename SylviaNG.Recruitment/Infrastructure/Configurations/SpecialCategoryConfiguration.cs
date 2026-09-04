using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class SpecialCategoryConfiguration : IEntityTypeConfiguration<SpecialCategory>
    {
        public void Configure(EntityTypeBuilder<SpecialCategory> builder)
        {
            builder.ToTable("SpecialCategories");
            builder.HasKey(s => s.SpecialCategoryId);

            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(s => s.Name).IsUnique();
        }
    }
}
