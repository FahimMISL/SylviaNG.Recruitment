using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");
            builder.HasKey(q => q.QuestionId);

            builder.Property(q => q.QuestionType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(q => q.QuestionText)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(q => q.Marks)
                .HasColumnType("decimal(5,2)");

            builder.Property(q => q.Explanation)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(q => q.QuestionGroupId);

            // Relationships
            builder.HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
