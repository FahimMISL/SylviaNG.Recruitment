using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class ReferralDuplicateConfiguration : IEntityTypeConfiguration<ReferralDuplicate>
    {
        public void Configure(EntityTypeBuilder<ReferralDuplicate> builder)
        {
            builder.ToTable("ReferralDuplicates");
            builder.HasKey(d => d.ReferralDuplicateId);

            builder.Property(d => d.MatchField)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Resolution)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(d => d.PrimaryReferralId);
            builder.HasIndex(d => d.DuplicateReferralId);

            // Relationships
            builder.HasOne(d => d.PrimaryReferral)
                .WithMany()
                .HasForeignKey(d => d.PrimaryReferralId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.DuplicateReferral)
                .WithMany()
                .HasForeignKey(d => d.DuplicateReferralId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.ResolvedByUser)
                .WithMany()
                .HasForeignKey(d => d.ResolvedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
