using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class InterviewSessionConfiguration : IEntityTypeConfiguration<InterviewSession>
    {
        public void Configure(EntityTypeBuilder<InterviewSession> builder)
        {
            builder.ToTable("InterviewSessions");
            builder.HasKey(s => s.InterviewSessionId);

            builder.Property(s => s.SessionTitle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Round)
                .HasMaxLength(100);

            builder.Property(s => s.Mode)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(s => s.MeetingLink)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(s => s.RequisitionId);
            builder.HasIndex(s => s.InterviewVenueId);

            // Relationships
            builder.HasOne(s => s.Requisition)
                .WithMany()
                .HasForeignKey(s => s.RequisitionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.InterviewVenue)
                .WithMany(v => v.InterviewSessions)
                .HasForeignKey(s => s.InterviewVenueId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Scorecard)
                .WithMany()
                .HasForeignKey(s => s.ScorecardId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Interviews)
                .WithOne(i => i.InterviewSession)
                .HasForeignKey(i => i.InterviewSessionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
