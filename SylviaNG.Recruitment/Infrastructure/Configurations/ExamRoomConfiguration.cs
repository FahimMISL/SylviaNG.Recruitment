using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamRoomConfiguration : IEntityTypeConfiguration<ExamRoom>
    {
        public void Configure(EntityTypeBuilder<ExamRoom> builder)
        {
            builder.ToTable("ExamRooms");
            builder.HasKey(r => r.ExamRoomId);

            builder.Property(r => r.RoomName)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(r => r.RoomName);
            builder.HasIndex(r => r.ExamVenueId);
        }
    }
}
