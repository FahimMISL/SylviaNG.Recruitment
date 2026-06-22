using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ShortlistFilterCriteriaConfiguration : IEntityTypeConfiguration<ShortlistFilterCriteria>
    {
        public void Configure(EntityTypeBuilder<ShortlistFilterCriteria> builder)
        {
            builder.ToTable("ShortlistFilterCriteria");
            builder.HasKey(c => c.ShortlistFilterCriteriaId);

            builder.Property(c => c.FieldName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Operator)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Value)
                .IsRequired()
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(c => c.ShortlistFilterId);
        }
    }
}
