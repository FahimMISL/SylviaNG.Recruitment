using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class JobPostingChannelConfiguration : IEntityTypeConfiguration<JobPostingChannel>
    {
        public void Configure(EntityTypeBuilder<JobPostingChannel> builder)
        {
            builder.ToTable("JobPostingChannels");
            builder.HasKey(c => c.JobPostingChannelId);

            builder.Property(c => c.Channel)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(c => c.PublishStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(c => c.ExternalReferenceId)
                .HasMaxLength(200);

            builder.Property(c => c.FailureReason)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(c => c.JobPostingId);
            builder.HasIndex(c => new { c.JobPostingId, c.Channel }).IsUnique();
        }
    }
}
