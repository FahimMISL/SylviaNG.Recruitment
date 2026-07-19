using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ExamQuestionOptionConfiguration : IEntityTypeConfiguration<ExamQuestionOption>
    {
        public void Configure(EntityTypeBuilder<ExamQuestionOption> builder)
        {
            builder.ToTable("ExamQuestionOptions");
            builder.HasKey(o => o.ExamQuestionOptionId);

            builder.Property(o => o.OptionText)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(o => o.ExamQuestionId);
        }
    }
}
