using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateSkillConfiguration : IEntityTypeConfiguration<CandidateSkill>
    {
        public void Configure(EntityTypeBuilder<CandidateSkill> builder)
        {
            builder.ToTable("CandidateSkills");
            builder.HasKey(s => s.CandidateSkillId);

            builder.Property(s => s.SkillName).IsRequired().HasMaxLength(100);
            builder.Property(s => s.ProficiencyLevel).HasMaxLength(20);

            builder.HasIndex(s => s.CandidateProfileId);

            builder.HasOne(s => s.CandidateProfile)
                .WithMany(c => c.Skills)
                .HasForeignKey(s => s.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.SkillLibraryItem)
                .WithMany()
                .HasForeignKey(s => s.SkillLibraryItemId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
