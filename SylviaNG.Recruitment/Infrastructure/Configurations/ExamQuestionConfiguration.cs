using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamQuestionConfiguration : IEntityTypeConfiguration<ExamQuestion>
    {
        public void Configure(EntityTypeBuilder<ExamQuestion> builder)
        {
            builder.ToTable("ExamQuestions");
            builder.HasKey(q => q.ExamQuestionId);

            builder.Property(q => q.QuestionText)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(q => q.QuestionType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(q => q.DifficultyLevel)
                .HasConversion<string>()
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(q => q.Marks)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(q => q.Explanation)
                .HasColumnType("text");

            builder.Property(q => q.ModelAnswer)
                .HasColumnType("text");

            builder.HasIndex(q => q.QuestionGroupId);
            builder.HasIndex(q => q.QuestionType);
            builder.HasIndex(q => q.DifficultyLevel);

            builder.HasMany(q => q.Options)
                .WithOne(o => o.ExamQuestion)
                .HasForeignKey(o => o.ExamQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
