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

            builder.Property(s => s.SkillName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.ProficiencyLevel)
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(s => s.CandidateId);
            builder.HasIndex(s => new { s.CandidateId, s.SkillName }).IsUnique();
        }
    }
}
