using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class JobPostingAttachmentConfiguration : IEntityTypeConfiguration<JobPostingAttachment>
    {
        public void Configure(EntityTypeBuilder<JobPostingAttachment> builder)
        {
            builder.ToTable("JobPostingAttachments");
            builder.HasKey(a => a.JobPostingAttachmentId);

            builder.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.StoredFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(a => a.JobPostingId);

            // Relationships
            builder.HasOne(a => a.JobPosting)
                .WithMany(j => j.Attachments)
                .HasForeignKey(a => a.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
