using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(p => p.PaymentId);

            builder.Property(p => p.Amount)
                .HasColumnType("numeric(18,2)");

            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(p => p.TransactionId)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(p => p.TransactionId)
                .IsUnique();

            builder.Property(p => p.GatewaySessionKey)
                .HasMaxLength(200);

            builder.Property(p => p.GatewayRedirectUrl)
                .HasMaxLength(1000);

            builder.Property(p => p.PaymentStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(p => p.ValidationId)
                .HasMaxLength(200);

            builder.Property(p => p.RawIpnPayload)
                .HasColumnType("text");

            builder.HasIndex(p => p.JobApplicationId);

            builder.HasOne(p => p.JobApplication)
                .WithMany(a => a.Payments)
                .HasForeignKey(p => p.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
