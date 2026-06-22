using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations;

public class ApplicationFeeConfiguration : IEntityTypeConfiguration<ApplicationFee>
{
    public void Configure(EntityTypeBuilder<ApplicationFee> builder)
    {
        builder.ToTable("ApplicationFees");
        builder.HasKey(e => e.ApplicationFeeId);

        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("BDT");
        builder.Property(e => e.PaymentMethods).HasMaxLength(500);
        builder.Property(e => e.WaiverRules).HasMaxLength(2000);

        builder.HasOne(e => e.JobPosting)
            .WithMany()
            .HasForeignKey(e => e.JobPostingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.JobPostingId);
    }
}
