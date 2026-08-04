using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamAnswerConfiguration : IEntityTypeConfiguration<ExamAnswer>
    {
        public void Configure(EntityTypeBuilder<ExamAnswer> builder)
        {
            builder.ToTable("ExamAnswers");
            builder.HasKey(a => a.ExamAnswerId);

            builder.Property(a => a.SelectedOptionIds).HasMaxLength(500);
            builder.Property(a => a.AnswerText).HasColumnType("text");
            builder.Property(a => a.MarksAwarded).HasColumnType("decimal(5,2)");

            builder.HasOne(a => a.ExamEnrollment)
                .WithMany(e => e.Answers)
                .HasForeignKey(a => a.ExamEnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.ExamQuestion)
                .WithMany()
                .HasForeignKey(a => a.ExamQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => new { a.ExamEnrollmentId, a.ExamQuestionId }).IsUnique();
        }
    }
}
