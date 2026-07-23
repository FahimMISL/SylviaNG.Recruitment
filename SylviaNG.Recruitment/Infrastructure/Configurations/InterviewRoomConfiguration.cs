using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class InterviewRoomConfiguration : IEntityTypeConfiguration<InterviewRoom>
    {
        public void Configure(EntityTypeBuilder<InterviewRoom> builder)
        {
            builder.ToTable("InterviewRooms");
            builder.HasKey(r => r.InterviewRoomId);

            builder.Property(r => r.RoomName)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(r => r.RoomName);
            builder.HasIndex(r => r.InterviewVenueId);
        }
    }
}
