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

            builder.Property(a => a.AnswerText)
                .HasColumnType("text");

            builder.Property(a => a.MarksAwarded)
                .HasColumnType("decimal(7,2)");

            // Indexes
            builder.HasIndex(a => a.ExamCandidateId);
            builder.HasIndex(a => a.QuestionId);

            // Relationships
            builder.HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.SelectedOption)
                .WithMany()
                .HasForeignKey(a => a.SelectedOptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
