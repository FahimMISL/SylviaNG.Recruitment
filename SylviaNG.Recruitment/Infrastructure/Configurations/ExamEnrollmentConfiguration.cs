using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamEnrollmentConfiguration : IEntityTypeConfiguration<ExamEnrollment>
    {
        public void Configure(EntityTypeBuilder<ExamEnrollment> builder)
        {
            builder.ToTable("ExamEnrollments");
            builder.HasKey(e => e.ExamEnrollmentId);

            builder.Property(e => e.SeatNumber).HasMaxLength(50);

            builder.Property(e => e.EmailNotificationStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.SmsNotificationStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.Score).HasColumnType("decimal(6,2)");

            builder.Property(e => e.ScoreSource)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(e => e.ScoredByUserName).HasMaxLength(150);

            builder.HasOne(e => e.Exam)
                .WithMany(x => x.Enrollments)
                .HasForeignKey(e => e.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.JobApplication)
                .WithMany()
                .HasForeignKey(e => e.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.ExamRoom)
                .WithMany()
                .HasForeignKey(e => e.ExamRoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.ExamId, e.JobApplicationId });
            builder.HasIndex(e => new { e.ExamId, e.ExamRoomId });
        }
    }
}
