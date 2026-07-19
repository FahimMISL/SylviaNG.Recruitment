using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class TalentPoolConfiguration : IEntityTypeConfiguration<TalentPool>
    {
        public void Configure(EntityTypeBuilder<TalentPool> builder)
        {
            builder.ToTable("TalentPools");
            builder.HasKey(t => t.TalentPoolId);

            builder.Property(t => t.Name).IsRequired().HasMaxLength(150);

            builder.HasIndex(t => t.Name).IsUnique();
        }
    }
}
