using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
    {
        public void Configure(EntityTypeBuilder<QuestionOption> builder)
        {
            builder.ToTable("QuestionOptions");
            builder.HasKey(o => o.QuestionOptionId);

            builder.Property(o => o.OptionText)
                .IsRequired()
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(o => o.QuestionId);
        }
    }
}
