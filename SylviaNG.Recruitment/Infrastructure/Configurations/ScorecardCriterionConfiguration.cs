using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ScorecardCriterionConfiguration : IEntityTypeConfiguration<ScorecardCriterion>
    {
        public void Configure(EntityTypeBuilder<ScorecardCriterion> builder)
        {
            builder.ToTable("ScorecardCriteria");
            builder.HasKey(c => c.ScorecardCriterionId);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Weight)
                .HasColumnType("decimal(6,2)")
                .IsRequired();

            builder.Property(c => c.MaxScore)
                .HasColumnType("decimal(6,2)")
                .IsRequired();

            builder.HasIndex(c => c.ScorecardId);
        }
    }
}
