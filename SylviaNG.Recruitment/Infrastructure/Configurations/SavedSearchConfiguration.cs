using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class SavedSearchConfiguration : IEntityTypeConfiguration<SavedSearch>
    {
        public void Configure(EntityTypeBuilder<SavedSearch> builder)
        {
            builder.ToTable("SavedSearches");
            builder.HasKey(s => s.SavedSearchId);

            builder.Property(s => s.SearchName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.QueryExpression)
                .IsRequired()
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(s => s.CreatedByUserId);

            // Relationships
            builder.HasOne(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
