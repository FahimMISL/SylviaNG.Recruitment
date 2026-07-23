using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ScorecardConfiguration : IEntityTypeConfiguration<Scorecard>
    {
        public void Configure(EntityTypeBuilder<Scorecard> builder)
        {
            builder.ToTable("Scorecards");
            builder.HasKey(s => s.ScorecardId);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Description)
                .HasMaxLength(1000);

            builder.HasIndex(s => s.Name);

            builder.HasMany(s => s.Criteria)
                .WithOne(c => c.Scorecard)
                .HasForeignKey(c => c.ScorecardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
