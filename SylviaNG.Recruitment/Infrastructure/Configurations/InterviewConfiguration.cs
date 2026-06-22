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

            builder.Property(i => i.Location)
                .HasMaxLength(300);

            builder.Property(i => i.MeetingLink)
                .HasMaxLength(500);

            builder.Property(i => i.Round)
                .HasMaxLength(100);

            builder.Property(i => i.Feedback)
                .HasColumnType("text");

            builder.Property(i => i.Result)
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(i => i.JobApplicationId);
            builder.HasIndex(i => i.InterviewSessionId);

            // Relationships
            builder.HasOne(i => i.InterviewSession)
                .WithMany(s => s.Interviews)
                .HasForeignKey(i => i.InterviewSessionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.Evaluations)
                .WithOne(e => e.Interview)
                .HasForeignKey(e => e.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
