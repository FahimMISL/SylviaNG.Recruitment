using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamVenueConfiguration : IEntityTypeConfiguration<ExamVenue>
    {
        public void Configure(EntityTypeBuilder<ExamVenue> builder)
        {
            builder.ToTable("ExamVenues");
            builder.HasKey(v => v.ExamVenueId);

            builder.Property(v => v.VenueName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(v => v.Location)
                .IsRequired()
                .HasMaxLength(300);

            builder.HasIndex(v => v.VenueName);

            builder.HasMany(v => v.Rooms)
                .WithOne(r => r.ExamVenue)
                .HasForeignKey(r => r.ExamVenueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
