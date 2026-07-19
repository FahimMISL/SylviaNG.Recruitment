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

            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(g => g.Description)
                .HasColumnType("text");

            builder.HasIndex(g => g.Name);

            builder.HasMany(g => g.Questions)
                .WithOne(q => q.QuestionGroup)
                .HasForeignKey(q => q.QuestionGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
