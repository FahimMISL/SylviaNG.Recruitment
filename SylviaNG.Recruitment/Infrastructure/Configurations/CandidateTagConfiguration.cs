using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class CandidateTagConfiguration : IEntityTypeConfiguration<CandidateTag>
    {
        public void Configure(EntityTypeBuilder<CandidateTag> builder)
        {
            builder.ToTable("CandidateTags");
            builder.HasKey(t => t.CandidateTagId);

            builder.Property(t => t.TagName).IsRequired().HasMaxLength(100);

            builder.HasIndex(t => t.CandidateProfileId);
            builder.HasIndex(t => t.TagName);

            builder.HasOne(t => t.CandidateProfile)
                .WithMany(c => c.Tags)
                .HasForeignKey(t => t.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
