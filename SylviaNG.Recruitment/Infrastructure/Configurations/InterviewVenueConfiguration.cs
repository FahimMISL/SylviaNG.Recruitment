using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class InterviewVenueConfiguration : IEntityTypeConfiguration<InterviewVenue>
    {
        public void Configure(EntityTypeBuilder<InterviewVenue> builder)
        {
            builder.ToTable("InterviewVenues");
            builder.HasKey(v => v.InterviewVenueId);

            builder.Property(v => v.VenueName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(v => v.Location)
                .IsRequired()
                .HasMaxLength(300);

            builder.HasIndex(v => v.VenueName);

            builder.HasMany(v => v.Rooms)
                .WithOne(r => r.InterviewVenue)
                .HasForeignKey(r => r.InterviewVenueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
