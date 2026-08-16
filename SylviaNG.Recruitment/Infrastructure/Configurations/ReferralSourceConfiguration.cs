using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ReferralSourceConfiguration : IEntityTypeConfiguration<ReferralSource>
    {
        public void Configure(EntityTypeBuilder<ReferralSource> builder)
        {
            builder.ToTable("ReferralSources");
            builder.HasKey(r => r.ReferralSourceId);

            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(r => r.Name).IsUnique();
        }
    }
}
