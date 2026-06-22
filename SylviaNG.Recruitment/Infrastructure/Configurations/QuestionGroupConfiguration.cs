using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class QuestionGroupConfiguration : IEntityTypeConfiguration<QuestionGroup>
    {
        public void Configure(EntityTypeBuilder<QuestionGroup> builder)
        {
            builder.ToTable("QuestionGroups");
            builder.HasKey(g => g.QuestionGroupId);

            builder.Property(g => g.GroupName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(g => g.Category)
                .HasMaxLength(100);

            builder.Property(g => g.DifficultyLevel)
                .HasMaxLength(50);

            // Relationships
            builder.HasMany(g => g.Questions)
                .WithOne(q => q.QuestionGroup)
                .HasForeignKey(q => q.QuestionGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
