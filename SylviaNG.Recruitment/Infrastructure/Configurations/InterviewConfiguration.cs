using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
    {
        public void Configure(EntityTypeBuilder<Interview> builder)
        {
            builder.ToTable("Interviews");
            builder.HasKey(i => i.InterviewId);

            builder.Property(i => i.MeetingLink)
                .HasMaxLength(500);

            builder.Property(i => i.CancellationReason)
                .HasMaxLength(500);

            builder.Property(i => i.EmailFailureReason)
                .HasMaxLength(500);

            builder.Property(i => i.Notes)
                .HasColumnType("text");

            builder.HasOne(i => i.InterviewVenue)
                .WithMany()
                .HasForeignKey(i => i.InterviewVenueId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.InterviewRoom)
                .WithMany()
                .HasForeignKey(i => i.InterviewRoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.InterviewRoundConfig)
                .WithMany()
                .HasForeignKey(i => i.InterviewRoundConfigId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(i => i.JobApplicationId);
            builder.HasIndex(i => i.InterviewRoomId);
            builder.HasIndex(i => i.InterviewRoundConfigId);
        }
    }
}
