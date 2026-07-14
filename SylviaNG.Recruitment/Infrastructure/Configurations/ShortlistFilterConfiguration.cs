using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ShortlistFilterConfiguration : IEntityTypeConfiguration<ShortlistFilter>
    {
        public void Configure(EntityTypeBuilder<ShortlistFilter> builder)
        {
            builder.ToTable("ShortlistFilters");
            builder.HasKey(f => f.ShortlistFilterId);

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(f => f.Description)
                .HasColumnType("text");

            builder.Property(f => f.CombineWith)
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.HasIndex(f => f.Name);

            builder.HasMany(f => f.Criteria)
                .WithOne(c => c.ShortlistFilter)
                .HasForeignKey(c => c.ShortlistFilterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
