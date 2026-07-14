using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ShortlistFilterCriterionConfiguration : IEntityTypeConfiguration<ShortlistFilterCriterion>
    {
        public void Configure(EntityTypeBuilder<ShortlistFilterCriterion> builder)
        {
            builder.ToTable("ShortlistFilterCriteria");
            builder.HasKey(c => c.ShortlistFilterCriterionId);

            builder.Property(c => c.CriterionType)
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(c => c.RequiredSkillNames)
                .HasMaxLength(1000);

            builder.Property(c => c.RequiredDistrict)
                .HasMaxLength(200);

            builder.HasIndex(c => new { c.ShortlistFilterId, c.DisplayOrder });
        }
    }
}
