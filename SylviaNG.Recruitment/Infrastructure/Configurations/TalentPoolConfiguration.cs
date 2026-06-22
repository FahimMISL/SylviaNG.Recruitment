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

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.RankingCriteriaJson)
                .HasColumnType("text");

            // Indexes
            builder.HasIndex(t => t.Name);
            builder.HasIndex(t => t.CreatedByUserId);

            // Relationships
            builder.HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.TalentPoolCandidates)
                .WithOne(tc => tc.TalentPool)
                .HasForeignKey(tc => tc.TalentPoolId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
