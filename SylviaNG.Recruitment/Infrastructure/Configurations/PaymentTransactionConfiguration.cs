using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Configurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions");
        builder.HasKey(e => e.PaymentTransactionId);

        builder.Property(e => e.TransactionId).IsRequired().HasMaxLength(64);
        builder.Property(e => e.SessionKey).HasMaxLength(256);
        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("BDT");
        builder.Property(e => e.PaymentStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(PaymentStatusEnum.Pending);
        builder.Property(e => e.GatewayResponse).HasMaxLength(4000);
        builder.Property(e => e.CardType).HasMaxLength(50);
        builder.Property(e => e.BankTransactionId).HasMaxLength(128);

        builder.HasOne(e => e.Candidate)
            .WithMany()
            .HasForeignKey(e => e.CandidateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.JobPosting)
            .WithMany()
            .HasForeignKey(e => e.JobPostingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ApplicationFee)
            .WithMany()
            .HasForeignKey(e => e.ApplicationFeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.TransactionId).IsUnique();
        builder.HasIndex(e => new { e.CandidateId, e.JobPostingId });
    }
}
