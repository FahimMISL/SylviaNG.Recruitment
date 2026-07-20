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
            builder.HasIndex(t => t.JobPostingId);

            // SetNull, not Restrict: Repository<T>.Delete is a hard delete, so a mandatory link
            // would permanently block deleting any JobPosting a pool ever referenced. Pools are
            // valid without a vacancy link, so losing the link on delete is the correct behavior.
            builder.HasOne(t => t.JobPosting)
                .WithMany()
                .HasForeignKey(t => t.JobPostingId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
