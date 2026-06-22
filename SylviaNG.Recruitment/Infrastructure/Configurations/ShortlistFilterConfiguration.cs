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

            builder.Property(f => f.FilterName)
                .IsRequired()
                .HasMaxLength(200);

            // Indexes
            builder.HasIndex(f => f.RequisitionId);

            // Relationships
            builder.HasOne(f => f.Requisition)
                .WithMany()
                .HasForeignKey(f => f.RequisitionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(f => f.Criteria)
                .WithOne(c => c.ShortlistFilter)
                .HasForeignKey(c => c.ShortlistFilterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
