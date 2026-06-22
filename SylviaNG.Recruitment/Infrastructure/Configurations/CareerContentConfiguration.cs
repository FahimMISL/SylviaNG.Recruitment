using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CareerContentConfiguration : IEntityTypeConfiguration<CareerContent>
    {
        public void Configure(EntityTypeBuilder<CareerContent> builder)
        {
            builder.ToTable("CareerContents");
            builder.HasKey(c => c.CareerContentId);

            builder.Property(c => c.ContentType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Body)
                .HasColumnType("text");

            builder.Property(c => c.MediaUrl)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(c => c.ContentType);
            builder.HasIndex(c => c.SortOrder);
        }
    }
}
