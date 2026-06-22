using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class RequisitionAttachmentConfiguration : IEntityTypeConfiguration<RequisitionAttachment>
    {
        public void Configure(EntityTypeBuilder<RequisitionAttachment> builder)
        {
            builder.ToTable("RequisitionAttachments");
            builder.HasKey(a => a.RequisitionAttachmentId);

            builder.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(a => a.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.ContentType)
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(a => a.RequisitionId);
        }
    }
}
