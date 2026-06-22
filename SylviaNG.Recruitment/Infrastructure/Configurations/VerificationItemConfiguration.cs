using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class VerificationItemConfiguration : IEntityTypeConfiguration<VerificationItem>
    {
        public void Configure(EntityTypeBuilder<VerificationItem> builder)
        {
            builder.ToTable("VerificationItems");
            builder.HasKey(i => i.VerificationItemId);

            builder.Property(i => i.VerificationType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(i => i.ItemStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(i => i.ReferenceNumber)
                .HasMaxLength(100);

            builder.Property(i => i.Notes)
                .HasColumnType("text");

            builder.Property(i => i.EvidenceFileUrl)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(i => i.VerificationWorkflowId);
            builder.HasIndex(i => i.VerificationType);

            // Relationships
            builder.HasOne(i => i.VerifiedByUser)
                .WithMany()
                .HasForeignKey(i => i.VerifiedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
