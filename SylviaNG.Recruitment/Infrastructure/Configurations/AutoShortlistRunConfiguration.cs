using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class AutoShortlistRunConfiguration : IEntityTypeConfiguration<AutoShortlistRun>
    {
        public void Configure(EntityTypeBuilder<AutoShortlistRun> builder)
        {
            builder.ToTable("AutoShortlistRuns");
            builder.HasKey(r => r.AutoShortlistRunId);

            builder.Property(r => r.Provider)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(r => new { r.JobPostingId, r.RunAt });

            builder.HasMany(r => r.Results)
                .WithOne(res => res.AutoShortlistRun)
                .HasForeignKey(res => res.AutoShortlistRunId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
