using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class WaiverRuleConfiguration : IEntityTypeConfiguration<WaiverRule>
    {
        public void Configure(EntityTypeBuilder<WaiverRule> builder)
        {
            builder.ToTable("WaiverRules");
            builder.HasKey(w => w.WaiverRuleId);

            builder.Property(w => w.Name).IsRequired().HasMaxLength(200);
            builder.Property(w => w.Description).HasMaxLength(1000);

            builder.Property(w => w.CandidateTypeFilter)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(w => w.Priority).IsRequired();
            builder.Property(w => w.IsActive).IsRequired();

            builder.HasIndex(w => w.Name).IsUnique();
            builder.HasIndex(w => w.Priority);

            builder.HasOne(w => w.SpecialCategory)
                .WithMany()
                .HasForeignKey(w => w.SpecialCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.ReferralSource)
                .WithMany()
                .HasForeignKey(w => w.ReferralSourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
