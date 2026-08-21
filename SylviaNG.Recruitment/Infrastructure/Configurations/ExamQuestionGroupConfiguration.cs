using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamQuestionGroupConfiguration : IEntityTypeConfiguration<ExamQuestionGroup>
    {
        public void Configure(EntityTypeBuilder<ExamQuestionGroup> builder)
        {
            builder.ToTable("ExamQuestionGroups");
            builder.HasKey(l => new { l.ExamId, l.QuestionGroupId });

            builder.HasOne(l => l.Exam)
                .WithMany(e => e.QuestionGroupLinks)
                .HasForeignKey(l => l.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.QuestionGroup)
                .WithMany()
                .HasForeignKey(l => l.QuestionGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
