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

            builder.Property(v => v.Address)
                .HasMaxLength(500);

            builder.Property(v => v.EquipmentDetails)
                .HasColumnType("text");

            builder.Property(v => v.ContactPerson)
                .HasMaxLength(200);

            builder.Property(v => v.ContactPhone)
                .HasMaxLength(50);

            // Relationships
            builder.HasMany(v => v.InterviewSessions)
                .WithOne(s => s.InterviewVenue)
                .HasForeignKey(s => s.InterviewVenueId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
