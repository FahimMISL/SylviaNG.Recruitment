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

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.OwnerUserName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.FilterJson)
                .IsRequired()
                .HasColumnType("text");

            builder.HasIndex(s => new { s.OwnerUserName, s.Name })
                .IsUnique();
        }
    }
}
